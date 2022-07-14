using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("load-schema", HelpText = "Execute SQL files provided in configuration against specified database.")]
public class LoadSchemaCommand : BaseCommand
{
    public async Task<int> Execute(SchemaWriter schemaWriter)
    {
        var config = ParseConfig();
        var connectionString = BuildConnectionString(config);
        var workingDirectory = Path.GetDirectoryName(ConfigurationFile);
        return await schemaWriter.ReadAndWriteSqlStatements(workingDirectory, connectionString, config);
    }
}