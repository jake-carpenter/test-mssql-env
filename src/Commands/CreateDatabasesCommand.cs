using CommandLine;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TestMssqlEnv.Commands;

[Verb("create-db", HelpText = "Create databases specified in configuration on the server.")]
public class CreateDatabasesCommand : BaseCommand
{
    public async Task<int> Execute()
    {
        var config = ParseConfig();
        await using var connection =
            new SqlConnection(
                $"Data Source=127.0.0.1,{config.Port};User Id=sa;Password={config.SaPassword};TrustServerCertificate=True;");
        
        foreach (var database in config.DatabasesToCreate)
        {
            await connection.ExecuteAsync($"CREATE DATABASE {database}");
        }

        return 0;
    }
}