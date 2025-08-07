using AccessData.Models;
using Authentication.Services;
using Microsoft.EntityFrameworkCore;
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

//base de datos declaraciones
var stringConnection = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<InventarioContext>(options =>
   options.UseSqlServer(stringConnection));
//base de datos declaraciones


//autneticacion declaraciones
builder.Services.AddScoped<TokenGeneratorContext>();
builder.Services.AddScoped<CtorTokenGeneratorRequest>();
builder.Services.AddScoped<TokenAdrian>();
builder.Services.AddScoped(typeof(ITokenGenerationStrategy), typeof(TokenAdrian));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//autneticacion declaraciones

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
