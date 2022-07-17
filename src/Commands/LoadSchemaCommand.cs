using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("load-schema", HelpText = "Execute SQL files provided in configuration against specified database.")]
public class LoadSchemaCommand : BaseCommand
{
    public async Task<int> Execute(SchemaWriter schemaWriter)
    {
        var config = ParseConfig();
        var workingDirectory = Path.GetDirectoryName(ConfigurationFile);

        await Logger.WrapWithOutputHeader(
            "Loading database schemas",
            () => schemaWriter.ReadAndWriteSqlStatements(workingDirectory!, config, this));

        return 0;
    }
}