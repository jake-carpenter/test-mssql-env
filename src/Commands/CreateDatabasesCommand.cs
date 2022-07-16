using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("create-db", HelpText = "Create databases specified in configuration on the server.")]
public class CreateDatabasesCommand : BaseCommand
{
    public async Task<int> Execute(SchemaWriter schemaWriter)
    {
        var config = ParseConfig();

        await Logger.WrapWithOutputHeader(
            "Create databases",
            () => schemaWriter.CreateDatabases(config));
        
        return 0;
    }
}