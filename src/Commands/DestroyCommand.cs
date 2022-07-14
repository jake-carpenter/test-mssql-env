using CommandLine;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

[Verb("destroy", HelpText = "Destroy the database container.")]
public class DestroyCommand : BaseCommand
{
    public Task<int> Execute(ContainerFactory databaseContainerFactory)
    {
        var config = ParseConfig();
        databaseContainerFactory.DestroyContainer(config);
        return Task.FromResult(0);
    }
}