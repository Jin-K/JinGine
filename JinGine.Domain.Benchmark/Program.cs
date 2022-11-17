using BenchmarkDotNet.Running;
using JinGine.Domain.Benchmark;

BenchmarkRunner.Run<FileContentCreateLinesBenchmarks>();
BenchmarkRunner.Run<FileContentConvertToPrintableTextBenchmarks>();