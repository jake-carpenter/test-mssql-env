namespace TestMssqlEnv.Infrastructure;

public class Config
{
    public string DockerImage { get; init; }
    public string SaPassword { get; init; }
    public string ContainerName { get; init; }
    public int Port { get; init; }
    public int HealthCheckTimeoutInSeconds { get; init; }
    public ICollection<string> DatabasesToCreate { get; init; } = new List<string>();
    public Dictionary<string, string[]> SqlFilesToExecute { get; init; } = new();
    
    public string ConnectionString => $"Data Source=127.0.0.1,{Port};User Id=sa;Password={SaPassword};TrustServerCertificate=True;";
}