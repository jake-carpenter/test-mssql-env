using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("destroy", HelpText = "Destroy the database container.")]
public class DestroyCommand : BaseCommand
{
    public Task<int> Execute(ContainerFactory containerFactory)
    {
        var config = ParseConfig();

        Logger.WrapWithOutputHeader(
            "Deleting any exising container",
            () => containerFactory.DestroyContainers(config));

        return Task.FromResult(0);
    }
}