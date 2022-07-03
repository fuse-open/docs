using System;

namespace Builder;

public class BuilderSettings
{
    public string RootPath { get; }
    public string OutputPath { get; }
    public bool GenerateReport { get; }
    public string BaseUrl { get; }
    public bool FastMode { get; }

    public BuilderSettings(string rootPath, string outputPath, bool generateReport, string baseUrl, bool fastMode)
    {
        RootPath = rootPath;
        OutputPath = outputPath;
        GenerateReport = generateReport;
        BaseUrl = baseUrl;
        FastMode = fastMode;
    }
}
