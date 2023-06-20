#nullable enable
using System;
using LBPUnion.ProjectLighthouse.Configuration.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace LBPUnion.ProjectLighthouse.Extensions;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, Action<YamlConfigurationSource>? configurationSource) =>
        builder.Add(configurationSource);

    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, ILighthouseConfiguration? lighthouseConfig, IFileProvider? provider, string path, bool optional, bool reloadOnChange)
    {
        return builder.AddYamlFile(s =>
        {
            s.FileProvider = provider;
            s.Path = path;
            s.Optional = optional;
            s.ReloadOnChange = reloadOnChange;
            s.Config = lighthouseConfig;
            s.ResolveFileProvider();
        });
    }

}