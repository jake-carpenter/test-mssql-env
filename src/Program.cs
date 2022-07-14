using CommandLine;
using TestMssqlEnv.Commands;
using TestMssqlEnv.Infrastructure;

var containerFactory = new ContainerFactory();
var schemaWriter = new SchemaWriter();

return await Parser.Default
    .ParseArguments<InitializeCommand, DestroyCommand, CreateDatabasesCommand, LoadSchemaCommand, ResetCommand>(args)
    .MapResult(
        (InitializeCommand command) => command.Execute(containerFactory),
        (DestroyCommand command) => command.Execute(containerFactory), 
        (CreateDatabasesCommand command) => command.Execute(schemaWriter),
        (LoadSchemaCommand command) => command.Execute(schemaWriter),
        (ResetCommand command) => command.Execute(containerFactory, schemaWriter),
        errors =>
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return Task.FromResult(1);
        });