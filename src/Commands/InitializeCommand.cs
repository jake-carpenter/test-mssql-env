using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("init", HelpText = "Initialize a MSSQL database container")]
public class InitializeCommand : BaseCommand
{
    public async Task<int> Execute(ContainerFactory containerFactory)
    {
        var config = ParseConfig();
        
        await Logger.WrapWithOutputHeader(
            "Initializing new container",
            () => containerFactory.StartContainer(config, this));
        
        await Logger.WrapWithOutputHeader(
            $"Waiting for healthy container (up to {config.HealthCheckTimeoutInSeconds} seconds)",
            () => containerFactory.VerifyContainerHealthy(config, this));

        return 0;
    }
}