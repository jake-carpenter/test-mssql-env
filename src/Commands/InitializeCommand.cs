using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("init", HelpText = "Initialize a MSSQL database container")]
public class InitializeCommand : BaseCommand
{
    public async Task<int> Execute(ContainerFactory containerFactory)
    {
        var config = ParseConfig(ConfigurationFile);

        await containerFactory.StartContainer(config);
        await containerFactory.VerifyContainerHealthy(config);

        return 0;
    }
}