
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method            | args   | Mean      | Error     | StdDev    | Median    | Code Size | Allocated |
|------------------ |------- |----------:|----------:|----------:|----------:|----------:|----------:|
| TypedDelegate_Get | class  | 3.6154 ns | 0.4670 ns | 1.3769 ns | 2.7530 ns |     358 B |         - |
| TypedDelegate_Set | class  |        NA |        NA |        NA |        NA |        NA |        NA |
| Direct_Get        | class  | 0.5973 ns | 0.0378 ns | 0.0353 ns | 0.5976 ns |     115 B |         - |
| Direct_Set        | class  | 2.4506 ns | 0.0527 ns | 0.0493 ns | 2.4545 ns |     125 B |         - |
| TypedDelegate_Get | struct | 2.4310 ns | 0.0571 ns | 0.0535 ns | 2.4427 ns |     370 B |         - |
| TypedDelegate_Set | struct |        NA |        NA |        NA |        NA |        NA |        NA |
| Direct_Get        | struct | 0.5712 ns | 0.0395 ns | 0.0369 ns | 0.5619 ns |     115 B |         - |
| Direct_Set        | struct | 0.2775 ns | 0.0391 ns | 0.0366 ns | 0.2769 ns |     124 B |         - |

Benchmarks with issues:
  PropertyAccessBenchmarks.TypedDelegate_Set: DefaultJob [args=class]
  PropertyAccessBenchmarks.TypedDelegate_Set: DefaultJob [args=struct]
