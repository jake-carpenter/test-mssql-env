using CommandLine;
using Spectre.Console;
using TestMssqlEnv.Commands;
using TestMssqlEnv.Infrastructure;

var containerFactory = new ContainerFactory();
var schemaWriter = new SchemaWriter();

return await Parser.Default
    .ParseArguments<InitializeCommand, DestroyCommand, CreateDatabasesCommand, LoadSchemaCommand, ResetCommand>(args)
    .MapResult(
        (InitializeCommand command) => ExceptionMiddleware(() => command.Execute(containerFactory), command),
        (DestroyCommand command) => ExceptionMiddleware(() => command.Execute(containerFactory), command),
        (CreateDatabasesCommand command) => ExceptionMiddleware(() => command.Execute(schemaWriter), command),
        (LoadSchemaCommand command) => ExceptionMiddleware(() => command.Execute(schemaWriter), command),
        (ResetCommand command) => ExceptionMiddleware(() => command.Execute(containerFactory, schemaWriter), command),
        errors =>
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return Task.FromResult(1);
        });

async Task<int> ExceptionMiddleware(Func<Task<int>> handler, BaseCommand command)
{
    try
    {
        return await handler();
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]FAILED: {ex.Message}[/]");
        AnsiConsole.WriteLine();

        if (command.Verbose)
        {
            AnsiConsole.MarkupLine("[yellow]EXCEPTION:[/]");
            AnsiConsole.WriteException(ex.InnerException ?? ex);
        }

        return 1;
    }
}