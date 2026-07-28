
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7548/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


| Method        | args         | Mean      | Error     | StdDev    | Gen0   | Code Size | Gen1   | Allocated |
|-------------- |------------- |----------:|----------:|----------:|-------:|----------:|-------:|----------:|
| GetAllMembers | cold-default |  8.344 μs | 0.1006 μs | 0.0941 μs | 3.4637 |  18,922 B |      - |   7.08 KB |
| GetAllMembers | cold-local   | 10.329 μs | 0.1199 μs | 0.1063 μs | 4.2725 |  25,081 B | 0.0153 |   8.79 KB |
| GetAllMembers | warm-default |  7.417 μs | 0.0748 μs | 0.0663 μs | 3.4409 |  14,875 B |      - |   7.04 KB |
| GetAllMembers | warm-local   |  7.847 μs | 0.1009 μs | 0.0944 μs | 3.4332 |  14,876 B |      - |   7.04 KB |
