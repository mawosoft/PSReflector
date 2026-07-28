
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method            | args   | Mean     | Error     | StdDev    | Code Size | Gen0   | Allocated |
|------------------ |------- |---------:|----------:|----------:|----------:|-------:|----------:|
| DynamicMethod_Get | class  | 2.530 ns | 0.0450 ns | 0.0421 ns |      16 B |      - |         - |
| DynamicMethod_Set | class  | 4.013 ns | 0.0471 ns | 0.0441 ns |      23 B |      - |         - |
| DynamicMethod_Get | struct | 7.153 ns | 0.0891 ns | 0.0834 ns |      16 B | 0.0115 |      24 B |
| DynamicMethod_Set | struct | 1.730 ns | 0.0205 ns | 0.0192 ns |      23 B |      - |         - |
