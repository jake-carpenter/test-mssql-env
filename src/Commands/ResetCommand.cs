using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("reset", HelpText = "Execute SQL files provided in configuration against specified database.")]
public class ResetCommand : BaseCommand
{
    public async Task<int> Execute(ContainerFactory containerFactory, SchemaWriter schemaWriter)
    {
        var config = ParseConfig();
        var connectionString = BuildConnectionString(config);
        var workingDirectory = Path.GetDirectoryName(ConfigurationFile);

        containerFactory.DestroyContainer(config);
        await containerFactory.StartContainer(config);
        await containerFactory.VerifyContainerHealthy(config);
        await schemaWriter.CreateDatabases(connectionString, config);
        await schemaWriter.ReadAndWriteSqlStatements(workingDirectory, connectionString, config);

        return 0;
    }
}