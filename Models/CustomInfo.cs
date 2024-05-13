namespace ManageServer.Models
{
    public class CustomInfo 
    {
        public GrpcSettings? GrpcSettings { get; set; }
        public RedisSettings? RedisSettings { get; set; }
    }

    public class GrpcSettings
    {
        public string? ServerIP { get; set; }
        public string? ClientIP { get; set; }
    }

    public class RedisSettings
    {
        public string? ConnectionString { get; set; }
    }
}
