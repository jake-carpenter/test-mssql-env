﻿using CommandLine;
using TestMssqlEnv.Commands;
using TestMssqlEnv.Infrastructure;

var containerFactory = new ContainerFactory();

return await Parser.Default
    .ParseArguments<InitializeCommand, DestroyCommand, CreateDatabasesCommand>(args)
    .MapResult(
        (InitializeCommand command) => command.Execute(containerFactory),
        (DestroyCommand command) => command.Execute(containerFactory), 
        (CreateDatabasesCommand command) => command.Execute(),
        errors =>
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return Task.FromResult(1);
        });