using Dapper;
using Microsoft.Data.SqlClient;

namespace TestMssqlEnv.Infrastructure;

public class SchemaWriter
{
    private readonly string[] _statementsToSkip =
    {
        "SET ANSI_PADDING ON", "SET ANSI_NULLS ON", "SET QUOTED_IDENTIFIER ON"
    };

    public async Task<IReadOnlyList<SqlStatements>> ReadSqlStatements(string workingDirectory, Config config)
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
                    .Where(sql => sql.Length > 0 && !_statementsToSkip.Contains(sql));

                commandsForDb.AddRange(sqlCommands);
            }

            sqlStatements.Add(new SqlStatements(database, commandsForDb));
        }

        return sqlStatements;
    }

    public async Task<int> WriteSqlStatements(string connectionString,  IReadOnlyCollection<SqlStatements> statements)
    {
        foreach (var statementSet in statements)
        {
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

    public async Task<int> ReadAndWriteSqlStatements(string workingDirectory, string connectionString, Config config)
    {
        var statements = await ReadSqlStatements(workingDirectory, config);
        return await WriteSqlStatements(connectionString, statements);
    }
}