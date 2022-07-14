using System.Reflection;

namespace TestMssqlEnv.Infrastructure;

public class Config
{
    public string DockerImage { get; init; }
    public string SaPassword { get; init; }
    public string ContainerName { get; init; }
    public int Port { get; init; }
    public ICollection<string> DatabasesToCreate { get; init; } = new List<string>();
    public Dictionary<string, string[]> SqlFilesToExecute { get; init; } = new();
}