using API_GATEWAY.Models;
using Encrypt.Resources;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text.Json;
using TokenGeneration.Context;
using TokenGeneration.Entidades;
using TokenGeneration.Interfaces;
using TokenGeneration.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TokenGeneratorContext>();
builder.Services.AddScoped<CtorTokenGeneratorRequest>();
builder.Services.AddScoped<TokenAdrian>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddScoped(typeof(ITokenGenerationStrategy), typeof(TokenAdrian));

builder.Services.AddSingleton<ITokenGenerationStrategy>(provider =>
{
    var secretKey = Key.key;
    return new TokenAdrian( new CtorTokenGeneratorRequest() { Algorithm = Encrypt.Enums.EncryptAlgorithm.AES, CypherMode = Encrypt.Enums.EncryptedMode.SHA256, SecretKey = Key.key })
    ;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
var endpointConfig = builder.Configuration.Get<EndpointConfiguration>();

app.UseEndpoints(endpoints =>
{
    foreach (var i in endpointConfig.Endpoints)
    {
        endpoints.MapMethods(i.Path, new[] { i.Method.ToUpperInvariant() }, async context =>
        {
            var tokenContext = context.RequestServices.GetRequiredService<TokenGeneratorContext>();
            await HandleRequest(context, i.TargetUri, tokenContext, i);
        });
    }
});


app.MapControllers();

app.Run();


//metodo que se usara en el mapmethods
async Task HandleRequest(HttpContext context, string targetUri, TokenGeneratorContext tokenGeneratorContext, API_GATEWAY.Models.Endpoint endpointConfiguration)
{
    string rol = string.Empty;
    int id = 0; 
    Console.Write("handdle request");
    if (Convert.ToBoolean(endpointConfiguration.AllowAuthentication))
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var tokenHeader))
        {
            if (string.IsNullOrWhiteSpace(Key.key))
                throw new InvalidOperationException("SecretKey no puede estar vacío");


            var authorizationContextString = context.Request.Headers.Authorization.ToString().Replace("Bearer", string.Empty).Trim();

            var tokenValidator = new TokenGeneratorContext(new TokenAdrian( new CtorTokenGeneratorRequest() { Algorithm = Encrypt.Enums.EncryptAlgorithm.AES, CypherMode = Encrypt.Enums.EncryptedMode.SHA256, SecretKey = Key.key}));
            var validToken = await tokenValidator.ValidateToken(authorizationContextString);
            rol = validToken.Data.Rol;  
            id = validToken.Data.UserId;

            if (!validToken.Data.IsValid)
            {
                throw (new Exception("No es un token valido"));
            }
        }
        else
        {
            throw (new Exception("No existe Autenticación en la invocación del método"));
        }
    }

    var client = app.Services.GetRequiredService<HttpClient>();


    // Generar el token usando el TokenGeneratorContext pasado como parámetro
    tokenGeneratorContext = new TokenGeneratorContext( new JWT( new CtorTokenGeneratorRequest() { Algorithm = Encrypt.Enums.EncryptAlgorithm.AES, CypherMode = Encrypt.Enums.EncryptedMode.SHA256, SecretKey = Key.key, Rol = rol , UserId = id} ));

    var token = await tokenGeneratorContext.GenerateToken(id);

    if (!token.Success || token.Data == null)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Failed to generate JWT token.");
        return;
    }

    //parte para los put y delete
    static string ResolveTemplateWithRouteValues(string template, RouteValueDictionary routeValues)
    {
        var resolved = template ?? "";
        foreach (var kv in routeValues)
        {
            var placeholder = "{" + kv.Key + "}";
            resolved = resolved.Replace(placeholder, kv.Value?.ToString() ?? "", StringComparison.OrdinalIgnoreCase);
        }
        return resolved;
    }
    var resolvedTarget = ResolveTemplateWithRouteValues(targetUri, context.Request.RouteValues);

    var ub = new UriBuilder(resolvedTarget);
    if (context.Request.QueryString.HasValue)
    {
        ub.Query = context.Request.QueryString.Value!.TrimStart('?');
    }
    var finalTargetUri = ub.Uri;

    //parte para los put y delete


    var requestMessage = new HttpRequestMessage
    {
        Method = new HttpMethod(context.Request.Method),
        RequestUri = finalTargetUri
    };

    var method = context.Request.Method;

    if (HttpMethods.IsPost(method) ||
        HttpMethods.IsPut(method) ||
        HttpMethods.IsPatch(method) ||
        (HttpMethods.IsDelete(method) && (context.Request.ContentLength ?? 0) > 0))
    {
        context.Request.EnableBuffering(); // permite leer sin “consumir” el stream

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var contentType = context.Request.ContentType ?? "application/json";
        requestMessage.Content = new StringContent(body, System.Text.Encoding.UTF8, contentType);
    }
    
    foreach (var header in context.Request.Headers)
    {
        if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
            continue;

        // Primero intenta como header general...
        if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
        {
            // ...si no, como header de contenido
            requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Data?.Token);

    var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

    if (!response.IsSuccessStatusCode)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var errorContent = await response.Content.ReadAsStringAsync();

        throw new Exception($"Error al ejecutar microservicio: {assemblyName} {response.StatusCode} ,Error: {errorContent}");
    }

    // Reenviar la respuesta del servicio de destino al cliente
    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
    {
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    foreach (var header in response.Content.Headers)
    {
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    context.Response.Headers.Remove("transfer-encoding");

    await response.Content.CopyToAsync(context.Response.Body);
}


