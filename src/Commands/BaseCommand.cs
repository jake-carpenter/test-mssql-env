using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

public abstract class BaseCommand
{
    protected static Config ParseConfig(string configurationFile)
    {
        var validPathToJsonRegex = new Regex("\\w+(\\.json)");
        if (!validPathToJsonRegex.IsMatch(configurationFile) )
            throw new ArgumentException("Invalid path to JSON configuration file provided with -c flag");
        
        var path = Path.Combine(Directory.GetCurrentDirectory(), configurationFile);
        var fileInfo = new FileInfo(path);

        if (!fileInfo.Exists)
            throw new ArgumentException($"Invalid configuration file provided: {fileInfo.FullName}");

        var configuration = new ConfigurationBuilder().AddJsonStream(fileInfo.OpenRead()).Build();
        var config = new Config();
        configuration.Bind(config);

        Console.WriteLine(configuration.GetDebugView());

        return config;
    }
}