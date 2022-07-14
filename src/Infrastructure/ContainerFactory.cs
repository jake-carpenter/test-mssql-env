using Dapper;
using Ductus.FluentDocker;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;
using Microsoft.Data.SqlClient;

namespace TestMssqlEnv.Infrastructure;

public class ContainerFactory
{
    public async Task StartContainer(Config config)
    {
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

        if (container.State != ServiceRunningState.Running)
        {
            container.Start();
        }
    }

    public async Task VerifyContainerHealthy(Config config)
    {
        var timeoutInSeconds = 30;
        var connectionString =
            $"Data Source=127.0.0.1,{config.Port};User Id=sa;Password={config.SaPassword};TrustServerCertificate=True;";

        var timeLimit = DateTime.Now.AddSeconds(timeoutInSeconds);
        await using var connection = new SqlConnection(connectionString);

        while (timeLimit > DateTime.Now)
        {
            Console.WriteLine("Trying to connect...");
            try
            {
                await connection.ExecuteAsync("SELECT 1");
                Console.WriteLine("Connected");
                return;
            }
            catch (Exception ex)
            {
                await Task.Delay(3000);
            }
        }

        throw new Exception("Database connection timeout exceeded.");
    }

    public void DestroyContainer(Config config)
    {
        var dockerServices = Fd.Discover();
        foreach (var dockerService in dockerServices)
        {
            var containers = dockerService.GetContainers(all: true);
            
            foreach (var container in containers)
            {
                if (container.Name != config.ContainerName)
                    continue;

                if (container.State == ServiceRunningState.Running)
                {
                    container.Stop();
                }

                container.Remove(force: true);
            }
        }
    }
}