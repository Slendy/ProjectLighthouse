#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using LBPUnion.ProjectLighthouse.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LBPUnion.ProjectLighthouse.Configuration.V2;

public class YamlConfigurationProvider : ConfigurationProvider
{
    private readonly YamlConfigurationSource source;

    public YamlConfigurationProvider(YamlConfigurationSource source)
    {
        this.source = source;
    }

    private void Load(Stream stream)
    {
        // Fuck a performance
        string textInput = new StreamReader(stream, detectEncodingFromByteOrderMarks: true).ReadToEnd();
        object? config = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build()
            .Deserialize(textInput, this.source.Config.GetType());
        if (config is not ILighthouseConfiguration lighthouseConfig) return;
        Console.WriteLine(@"stored config version: " + lighthouseConfig.ConfigVersion);
        Console.WriteLine(@"latest config version: " + this.source.Config.ConfigVersion);

        try
        {
            this.Data = new YamlConfigurationStreamParser().Parse(textInput);
        }
        catch (YamlException e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine(this.Data.Count);
        foreach (KeyValuePair<string, string?> kvp in this.Data)
        {
            Console.WriteLine($@"{kvp.Key}: {kvp.Value}");
        }
    }

    public override void Load()
    {
        if (this.source.Path == null) throw new ArgumentNullException(nameof(this.source.Path));
        if (this.source.FileProvider == null) throw new ArgumentNullException(nameof(this.source.FileProvider));

        Console.WriteLine($@"Load config: {this.source.Path}");
        IFileInfo? file = this.source.FileProvider?.GetFileInfo(this.source.Path ?? string.Empty);

        Console.WriteLine(@"fileinfo: " + (file?.ToString() ?? "null"));
        Console.WriteLine(@"Exists: " + (file?.Exists.ToString() ?? "false"));

        if (!file?.Exists ?? false)
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            string serializedDefaultConfig = serializer.Serialize(this.source.Config);
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, this.source.Path ?? string.Empty), serializedDefaultConfig);
            this.source.ResolveFileProvider();
        }

        try
        {
            if (file?.PhysicalPath == null)
            {
                throw new ArgumentNullException(nameof(file.PhysicalPath));
            }

            using Stream fileStream = file.CreateReadStream();
            this.Load(fileStream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToDetailedException());
        }
    }
}