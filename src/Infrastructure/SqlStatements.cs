using System.Collections;

namespace TestMssqlEnv.Infrastructure;

public class SqlStatements : IEnumerable<string>
{
    private readonly IEnumerable<string> _statements;
    public SqlStatements(string database, IEnumerable<string> statements)
    {
        Database = database;
        _statements = statements;
    }
    
    public string Database { get; }
    public IEnumerator<string> GetEnumerator() => _statements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}