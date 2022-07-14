using System.Text.RegularExpressions;
using CommandLine;
using Microsoft.Extensions.Configuration;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

public abstract class BaseCommand
{
    [Option('c', "configuration", HelpText = "Relative or full path to a configuration file", Required = true)]
    public string ConfigurationFile { get; init; }
    
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