using CommandLine;
using Spectre.Console;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("reset", HelpText = "Execute SQL files provided in configuration against specified database.")]
public class ResetCommand : BaseCommand
{
    public async Task<int> Execute(ContainerFactory containerFactory, SchemaWriter schemaWriter)
    {
        var config = ParseConfig();
        var workingDirectory = Path.GetDirectoryName(ConfigurationFile);

        await AnsiConsole.Status()
            .StartAsync("Resetting test database environment...", async _ =>
            {
                Logger.WrapWithOutputHeader(
                    "Deleting any exising container",
                    () => containerFactory.DestroyContainers(config));

                await Logger.WrapWithOutputHeader(
                    "Initializing new container",
                    () => containerFactory.StartContainer(config, this));

                await Logger.WrapWithOutputHeader(
                    $"Waiting for healthy container (up to {config.HealthCheckTimeoutInSeconds} seconds)",
                    () => containerFactory.VerifyContainerHealthy(config, this));

                await Logger.WrapWithOutputHeader(
                    "Creating databases",
                    () => schemaWriter.CreateDatabases(config));

                await Logger.WrapWithOutputHeader(
                    "Loading database schemas",
                    () => schemaWriter.ReadAndWriteSqlStatements(workingDirectory, config, this));
            });


        return 0;
    }
}