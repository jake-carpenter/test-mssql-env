using System.Reflection;

namespace TestMssqlEnv.Infrastructure;

public class Config
{
    public string DockerImage { get; init; }
    public string SaPassword { get; init; }
    public string ContainerName { get; init; }
    public int Port { get; init; }
}