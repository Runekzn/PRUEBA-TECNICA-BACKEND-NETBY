namespace API_GATEWAY.Models
{
    public class Endpoint
    {
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty ;
        public string TargetUri { get; set; } = string.Empty;
        public bool AllowAuthentication { get; set; }
    }

    public class EndpointConfiguration
    {
        public List<Endpoint> Endpoints { get; set; } = new();
    }
}
