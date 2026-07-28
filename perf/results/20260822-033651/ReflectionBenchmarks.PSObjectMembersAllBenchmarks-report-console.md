
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7548/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.302
  [Host]             : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9339.0), X64 RyuJIT VectorSize=256

Job=.NET Framework 4.8  Runtime=.NET Framework 4.8  Toolchain=net48  

| Method        | args         | Mean     | Error    | StdDev   | Gen0   | Allocated |
|-------------- |------------- |---------:|---------:|---------:|-------:|----------:|
| GetAllMembers | cold-default | 43.24 μs | 0.556 μs | 0.465 μs | 4.6997 |   9.75 KB |
| GetAllMembers | cold-local   | 44.90 μs | 0.402 μs | 0.336 μs | 6.1035 |  12.54 KB |
| GetAllMembers | warm-default | 41.87 μs | 0.462 μs | 0.432 μs | 4.6997 |   9.71 KB |
| GetAllMembers | warm-local   | 40.64 μs | 0.387 μs | 0.343 μs | 4.6997 |   9.71 KB |
