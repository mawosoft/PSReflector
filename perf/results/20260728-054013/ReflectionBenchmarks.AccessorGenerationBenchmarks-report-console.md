
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method                      | args   | Mean      | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|---------------------------- |------- |----------:|---------:|---------:|-------:|-------:|----------:|
| LinqExpression_Getter_Field | class  | 117.38 μs | 1.488 μs | 1.319 μs | 2.1973 | 1.9531 |   4.68 KB |
| LinqExpression_Setter_Field | class  | 132.35 μs | 1.644 μs | 1.457 μs | 2.1973 | 1.9531 |   4.92 KB |
| DynamicMethod_Getter_Field  | class  |  96.64 μs | 1.137 μs | 1.008 μs | 0.6104 | 0.4883 |   1.27 KB |
| LinqExpression_Getter_Field | struct | 134.76 μs | 2.693 μs | 4.499 μs | 2.1973 | 1.9531 |   4.68 KB |
| LinqExpression_Setter_Field | struct | 150.00 μs | 1.275 μs | 1.065 μs | 2.1973 | 1.9531 |   4.92 KB |
| DynamicMethod_Getter_Field  | struct | 115.83 μs | 1.881 μs | 1.759 μs | 0.6104 | 0.4883 |   1.27 KB |
