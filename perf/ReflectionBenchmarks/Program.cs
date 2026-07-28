// Copyright (c) Matthias Wolf, Mawosoft.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace ReflectionBenchmarks;

internal static class Program
{
    internal static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance);
        (config.GetExporters() as List<IExporter>)?.Clear();
        config = config
            .AddExporter(MarkdownExporter.Console)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .WithOption(ConfigOptions.DisableOptimizationsValidator, true)
            //.WithOption(ConfigOptions.KeepBenchmarkFiles, true)
            .WithOption(ConfigOptions.DontOverwriteResults, true);
        var debuggable = typeof(Program).Assembly.GetCustomAttribute<DebuggableAttribute>();
        if (debuggable is not null && debuggable.IsJITOptimizerDisabled)
        {
            config = config
                .AddJob(Job.InProcess.WithStrategy(RunStrategy.ColdStart).WithIterationCount(1));
            args = ["-f", "*"];
            _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
        else
        {
            _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}
