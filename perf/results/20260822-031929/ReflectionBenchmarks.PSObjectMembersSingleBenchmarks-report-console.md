
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7548/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


| Method          | args         | Mean     | Error   | StdDev  | Gen0   | Code Size | Allocated |
|---------------- |------------- |---------:|--------:|--------:|-------:|----------:|----------:|
| GetSingleMember | cold-default | 247.1 ns | 2.41 ns | 2.25 ns | 0.1488 |   3,184 B |     312 B |
| GetSingleMember | cold-local   | 221.9 ns | 1.24 ns | 1.10 ns | 0.1490 |   3,166 B |     312 B |
| GetSingleMember | warm-default | 240.3 ns | 2.61 ns | 2.32 ns | 0.1297 |   4,491 B |     272 B |
| GetSingleMember | warm-local   | 244.2 ns | 3.21 ns | 2.85 ns | 0.1297 |   4,617 B |     272 B |
