
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7548/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


| Method              | args    | Mean     | Error    | StdDev   | Gen0   | Code Size | Allocated |
|-------------------- |-------- |---------:|---------:|---------:|-------:|----------:|----------:|
| AsPSObject_Public   | ?       | 20.05 ns | 0.253 ns | 0.225 ns | 0.0727 |     630 B |     152 B |
| AsPSObject_Internal | default | 22.09 ns | 0.281 ns | 0.250 ns | 0.0727 |      56 B |     152 B |
| AsPSObject_Internal | local   | 22.28 ns | 0.293 ns | 0.274 ns | 0.0727 |      56 B |     152 B |
