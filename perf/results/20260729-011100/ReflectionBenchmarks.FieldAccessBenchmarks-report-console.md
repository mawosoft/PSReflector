
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method     | args   | Mean      | Error     | StdDev    | Code Size | Allocated |
|----------- |------- |----------:|----------:|----------:|----------:|----------:|
| Direct_Get | class  | 0.5376 ns | 0.0286 ns | 0.0267 ns |     115 B |         - |
| Direct_Set | class  | 2.4089 ns | 0.0462 ns | 0.0432 ns |     125 B |         - |
| Direct_Get | struct | 0.5649 ns | 0.0237 ns | 0.0221 ns |     115 B |         - |
| Direct_Set | struct | 0.3045 ns | 0.0260 ns | 0.0243 ns |     124 B |         - |
