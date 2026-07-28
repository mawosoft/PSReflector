
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7548/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.302
  [Host]             : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9339.0), X64 RyuJIT VectorSize=256

Job=.NET Framework 4.8  Runtime=.NET Framework 4.8  Toolchain=net48  

| Method              | args    | Mean     | Error    | StdDev   | Gen0   | Allocated |
|-------------------- |-------- |---------:|---------:|---------:|-------:|----------:|
| AsPSObject_Public   | ?       | 44.55 ns | 0.540 ns | 0.505 ns | 0.0727 |     152 B |
| AsPSObject_Internal | default | 44.34 ns | 0.513 ns | 0.454 ns | 0.0727 |     152 B |
| AsPSObject_Internal | local   | 46.43 ns | 0.604 ns | 0.565 ns | 0.0727 |     152 B |
