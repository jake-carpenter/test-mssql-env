using Dapper;
using Ductus.FluentDocker;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;
using Microsoft.Data.SqlClient;
using Spectre.Console;
using TestMssqlEnv.Commands;

namespace TestMssqlEnv.Infrastructure;

public class ContainerFactory
{
    public async Task StartContainer(Config config, BaseCommand command)
    {
        AnsiConsole.MarkupLine("Creating container:");
        AnsiConsole.MarkupLine($"\tImage:\t[yellow]{config.DockerImage}[/]");
        AnsiConsole.MarkupLine($"\tName:\t[yellow]{config.ContainerName}[/]");
        AnsiConsole.MarkupLine($"\tPort:\t[yellow]{config.Port.ToString()}[/]");

        var container = new Builder()
            .UseContainer()
            .UseImage(config.DockerImage)
            .KeepContainer()
            .KeepRunning()
            .WithEnvironment("ACCEPT_EULA=Y", $"MSSQL_SA_PASSWORD={config.SaPassword}")
            .WithName(config.ContainerName)
            .ReuseIfExists()
            .ExposePort(config.Port, 1433)
            .Build();
        
        Logger.MarkupLineWhenVerbose($"[grey]\t Id:\t{container.Id}[/]", command.Verbose);

        if (container.State != ServiceRunningState.Running)
        {
            Logger.MarkupLineWhenVerbose("Starting container...", command.Verbose);
            container.Start();
            container.WaitForRunning();
            Logger.MarkupLineWhenVerbose("[green]Done[/]", command.Verbose);
        }
    }

    public async Task VerifyContainerHealthy(Config config, BaseCommand command)
    {
        var failures = 0;
        var timeLimit = DateTime.Now.AddSeconds(config.HealthCheckTimeoutInSeconds);
        Exception latestException = null;
        await using var connection = new SqlConnection(config.ConnectionString);

        while (timeLimit > DateTime.Now)
        {
            await Task.Delay(3000);

            try
            {
                await connection.ExecuteAsync("SELECT 1");
                AnsiConsole.MarkupLine("Container is [green]healthy[/]");
                return;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[fuchsia]Failed attempt #{++failures}[/]");
                Logger.MarkupLineWhenVerbose($"[grey]{ex.Message.TrimEnd()}[/]", command.Verbose);
                latestException = ex;
            }
        }

        AnsiConsole.WriteException(latestException!, ExceptionFormats.ShortenEverything);
        throw new Exception("Database connection timeout exceeded.");
    }

    public IEnumerable<IContainerService> FindExistingContainer(Config config)
    {
        var dockerServices = Fd.Discover();
        foreach (var dockerService in dockerServices)
        {
            var containers = dockerService.GetContainers(all: true);

            foreach (var container in containers)
            {
                if (container.Name != config.ContainerName)
                    continue;

                yield return container;
            }
        }
    }

    public void DestroyContainers(Config config)
    {
        var containers = FindExistingContainer(config).ToArray();
        AnsiConsole.MarkupLine($"Found {containers.Length} existing container(s) to destroy");

        foreach (var container in containers)
        {
            if (container.State == ServiceRunningState.Running)
            {
                AnsiConsole.MarkupLine($"Stopping [yellow]{container.Name}[/]...");
                container.Stop();
            }

            AnsiConsole.MarkupLine($"Destroying [yellow]{container.Name}[/]...");
            container.Remove(force: true);
            AnsiConsole.MarkupLine("[green]Done[/]");
        }
    }
}