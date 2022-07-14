using System.Text.RegularExpressions;
using CommandLine;
using Microsoft.Extensions.Configuration;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

public abstract class BaseCommand
{
    [Option('c', "configuration", HelpText = "Relative or full path to a configuration file", Required = true)]
    public string ConfigurationFile { get; init; }
    
    protected Config ParseConfig()
    {
        var validPathToJsonRegex = new Regex("\\w+(\\.json)");
        if (!validPathToJsonRegex.IsMatch(ConfigurationFile) )
            throw new ArgumentException("Invalid path to JSON configuration file provided with -c flag");
        
        var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFile);
        var fileInfo = new FileInfo(path);

        if (!fileInfo.Exists)
            throw new ArgumentException($"Invalid configuration file provided: {fileInfo.FullName}");

        using var stream = fileInfo.OpenRead();
        var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
        var config = new Config();
        configuration.Bind(config);

        Console.WriteLine(configuration.GetDebugView());

        return config;
    }

    protected static string BuildConnectionString(Config config)
    {
        return
            $"Data Source=127.0.0.1,{config.Port};User Id=sa;Password={config.SaPassword};TrustServerCertificate=True;";
    }
}