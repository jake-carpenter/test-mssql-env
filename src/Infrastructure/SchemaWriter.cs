using Dapper;
using Microsoft.Data.SqlClient;
using Spectre.Console;
using TestMssqlEnv.Commands;

namespace TestMssqlEnv.Infrastructure;

public class SchemaWriter
{
    private readonly string[] _statementsToSkip =
    {
        "SET ANSI_PADDING ON", "SET ANSI_NULLS ON", "SET QUOTED_IDENTIFIER ON"
    };

    public async Task CreateDatabases(Config config)
    {
        await using var connection = new SqlConnection(config.ConnectionString);

        foreach (var database in config.DatabasesToCreate)
        {
            AnsiConsole.MarkupLine($"Creating database: [yellow]{database}[/]");
            await connection.ExecuteAsync($"CREATE DATABASE {database}");
        }
    }

    public async Task<IReadOnlyList<SqlStatements>> ReadSqlStatements(
        string workingDirectory,
        Config config,
        BaseCommand command)
    {
        var sqlStatements = new List<SqlStatements>();

        foreach (var (database, files) in config.SqlFilesToExecute)
        {
            var commandsForDb = new List<string>();
            foreach (var file in files)
            {
                var path = Path.Combine(workingDirectory, file);
                var fileInfo = new FileInfo(path);

                if (!fileInfo.Exists)
                    throw new Exception("SQL file doesn't exist");

                await using var stream = fileInfo.OpenRead();
                using var streamReader = new StreamReader(stream);
                var text = await streamReader.ReadToEndAsync();
                var sqlCommands = text
                    .Split("GO")
                    .Select(sql => sql.Trim())
                    .Where(sql => sql.Length > 0 && !_statementsToSkip.Contains(sql))
                    .ToArray();

                commandsForDb.AddRange(sqlCommands);
                Logger.MarkupLineWhenVerbose(
                    $"[grey]Read {sqlCommands.Length} SQL statements from file: {fileInfo.Name}[/]", command.Verbose);
            }

            sqlStatements.Add(new SqlStatements(database, commandsForDb));
        }

        return sqlStatements;
    }

    public async Task<int> WriteSqlStatements(string connectionString, IReadOnlyList<SqlStatements> statements)
    {
        foreach (var statementSet in statements)
        {
            AnsiConsole.MarkupLine(
                $"Executing {statementSet.Count()} SQL statements on [yellow]{statementSet.Database}[/]");

            var connectionStringWithDb = $"{connectionString};Database={statementSet.Database}";
            await using var connection = new SqlConnection(connectionStringWithDb);
            await connection.OpenAsync();

            foreach (var statement in statementSet)
            {
                await connection.ExecuteAsync(statement);
            }
        }

        return 0;
    }

    public async Task<int> ReadAndWriteSqlStatements(string workingDirectory, Config config, BaseCommand command)
    {
        var statements = await ReadSqlStatements(workingDirectory, config, command);
        return await WriteSqlStatements(config.ConnectionString, statements);
    }
}