
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method          | args   | Mean     | Error    | StdDev   | Gen0   | Allocated |
|---------------- |------- |---------:|---------:|---------:|-------:|----------:|
| PSReflector_Get | class  | 15.73 ns | 0.213 ns | 0.189 ns |      - |         - |
| PSReflector_Get | struct | 20.75 ns | 0.293 ns | 0.229 ns | 0.0115 |      24 B |
