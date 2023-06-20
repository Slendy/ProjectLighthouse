using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using YamlDotNet.RepresentationModel;

namespace LBPUnion.ProjectLighthouse.Configuration.V2;

internal class YamlConfigurationStreamParser
{
    private readonly IDictionary<string, string> data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> ctx = new();
    private string currentPath;

    public IDictionary<string, string> Parse(string input)
    {
        this.data.Clear();
        this.ctx.Clear();

        // https://dotnetfiddle.net/rrR2Bb
        YamlStream yaml = new();
        yaml.Load(new StringReader(input));

        if (!yaml.Documents.Any()) return this.data;

        YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

        // The document node is a mapping node
        this.VisitYamlMappingNode(mapping);

        return this.data;
    }

    private void VisitYamlNodePair(KeyValuePair<YamlNode, YamlNode> yamlNodePair)
    {
        string context = ((YamlScalarNode)yamlNodePair.Key).Value;
        this.VisitYamlNode(context, yamlNodePair.Value);
    }

    private void VisitYamlNode(string context, YamlNode node)
    {
        switch (node)
        {
            case YamlScalarNode scalarNode: this.VisitYamlScalarNode(context, scalarNode);
                break;
            case YamlMappingNode mappingNode: this.VisitYamlMappingNode(context, mappingNode);
                break;
            case YamlSequenceNode sequenceNode: this.VisitYamlSequenceNode(context, sequenceNode);
                break;
        }
    }

    private void VisitYamlScalarNode(string context, YamlScalarNode yamlValue)
    {
        //a node with a single 1-1 mapping 
        this.EnterContext(context);
        string currentKey = this.currentPath;

        if (this.data.ContainsKey(currentKey))
        {
            throw new FormatException();
        }

        this.data[currentKey] = IsNullValue(yamlValue) ? null : yamlValue.Value;
        this.ExitContext();
    }

    private void VisitYamlMappingNode(YamlMappingNode node)
    {
        foreach (KeyValuePair<YamlNode, YamlNode> yamlNodePair in node.Children)
        {
            this.VisitYamlNodePair(yamlNodePair);
        }
    }

    private void VisitYamlMappingNode(string context, YamlMappingNode yamlValue)
    {
        //a node with an associated sub-document
        this.EnterContext(context);

        this.VisitYamlMappingNode(yamlValue);

        this.ExitContext();
    }

    private void VisitYamlSequenceNode(string context, YamlSequenceNode yamlValue)
    {
        // a node with an associated list
        this.EnterContext(context);

        this.VisitYamlSequenceNode(yamlValue);

        this.ExitContext();
    }

    private void VisitYamlSequenceNode(YamlSequenceNode node)
    {
        for (int i = 0; i < node.Children.Count; i++)
        {
            this.VisitYamlNode(i.ToString(), node.Children[i]);
        }
    }

    private void EnterContext(string context)
    {
        this.ctx.Push(context);
        this.currentPath = ConfigurationPath.Combine(this.ctx.Reverse());
    }

    private void ExitContext()
    {
        this.ctx.Pop();
        this.currentPath = ConfigurationPath.Combine(this.ctx.Reverse());
    }

    private static bool IsNullValue(YamlScalarNode yamlValue) => yamlValue.Style == YamlDotNet.Core.ScalarStyle.Plain && yamlValue.Value is "~" or "null" or "Null" or "NULL";
}