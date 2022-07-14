using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("init", HelpText = "Initialize a MSSQL database container")]
public class InitializeDatabaseCommand : BaseCommand
{
    [Option('c', "configuration", HelpText = "Relative or full path to a configuration file", Required = true)]
    public string ConfigurationFile { get; init; }
    
    public async Task<int> Execute(ContainerFactory containerFactory)
    {
        var config = ParseConfig(ConfigurationFile);

        await containerFactory.StartContainer(config);
        await containerFactory.VerifyContainerHealthy(config);

        return 0;
    }
}