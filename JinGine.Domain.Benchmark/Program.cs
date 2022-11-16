using BenchmarkDotNet.Running;
using JinGine.Domain.Benchmark;

BenchmarkRunner.Run<FileContentBenchmarks>();
BenchmarkRunner.Run<FileContentTextLineLoopBenchmarks>();
BenchmarkRunner.Run<FileContentTextLineArrayCreationBenchmarks>();