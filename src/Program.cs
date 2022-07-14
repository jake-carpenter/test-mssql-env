using CommandLine;
using TestMssqlEnv.Commands;
using TestMssqlEnv.Infrastructure;

var containerFactory = new ContainerFactory();

return await Parser.Default
    .ParseArguments<InitializeDatabaseCommand>(args)
    .MapResult(command => command.Execute(containerFactory), errors =>
    {
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }

        return Task.FromResult(1);
    });
