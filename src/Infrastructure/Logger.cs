using Spectre.Console;

namespace TestMssqlEnv.Infrastructure;

public static class Logger
{
    private static readonly Rule _rule = new Rule { Alignment = Justify.Left, Style = Style.Parse("blue") };

    public static void WrapWithOutputHeader(string title, Action action)
    {
        _rule.Title = title;
        AnsiConsole.Write(_rule);
        action();
        AnsiConsole.WriteLine();
    }
    
    public static async Task WrapWithOutputHeader(string title, Func<Task> action)
    {
        _rule.Title = title;
        AnsiConsole.Write(_rule);
        await action();
        AnsiConsole.WriteLine();
    }
    
    public static void MarkupLineWhenVerbose(string message, bool verbose)
    {
        if (verbose)
        {
            AnsiConsole.MarkupLine(message);
        }
    }
}