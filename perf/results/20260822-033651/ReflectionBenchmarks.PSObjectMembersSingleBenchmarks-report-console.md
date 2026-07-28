
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7548/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.302
  [Host]             : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9339.0), X64 RyuJIT VectorSize=256

Job=.NET Framework 4.8  Runtime=.NET Framework 4.8  Toolchain=net48  

| Method          | args         | Mean     | Error     | StdDev    | Gen0   | Allocated |
|---------------- |------------- |---------:|----------:|----------:|-------:|----------:|
| GetSingleMember | cold-default | 1.633 μs | 0.0175 μs | 0.0164 μs | 0.1717 |     361 B |
| GetSingleMember | cold-local   | 1.842 μs | 0.0109 μs | 0.0102 μs | 0.1717 |     361 B |
| GetSingleMember | warm-default | 1.589 μs | 0.0125 μs | 0.0117 μs | 0.1526 |     321 B |
| GetSingleMember | warm-local   | 1.579 μs | 0.0115 μs | 0.0107 μs | 0.1526 |     321 B |
