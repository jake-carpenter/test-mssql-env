using System.Text.Json;
using System.Text.RegularExpressions;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using TestMssqlEnv.Infrastructure;

namespace TestMssqlEnv.Commands;

public abstract class BaseCommand
{
    [Option('c', "configuration", HelpText = "Relative or full path to a configuration file", Required = true)]
    public string ConfigurationFile { get; init; }

    [Option('v', "verbose", HelpText = "Set output to verbose messages")]
    public bool Verbose { get; init; }

    protected Config ParseConfig()
    {
        var validPathToJsonRegex = new Regex("\\w+(\\.json)");
        if (!validPathToJsonRegex.IsMatch(ConfigurationFile))
            throw new ArgumentException("Invalid path to JSON configuration file provided with -c flag");

        var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFile);
        var fileInfo = new FileInfo(path);

        if (!fileInfo.Exists)
            throw new ArgumentException($"Invalid configuration file provided: {fileInfo.FullName}");

        using var stream = fileInfo.OpenRead();
        var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
        var config = new Config();
        configuration.Bind(config);

        MaybeOutputConfig(config, fileInfo.FullName);

        return config;
    }

    private void MaybeOutputConfig(Config config, string configurationPath)
    {
        if (!Verbose)
            return;

        AnsiConsole.Write(
            new Rule
            {
                Alignment = Justify.Left, Style = Style.Parse("blue"),
                Title = $"Reading configuration from {configurationPath}"
            });

        var json = JsonSerializer.Serialize(config,
            new JsonSerializerOptions
            {
                WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreReadOnlyProperties = true
            });

        AnsiConsole.MarkupLineInterpolated($"{json}");
        AnsiConsole.WriteLine();
    }
}