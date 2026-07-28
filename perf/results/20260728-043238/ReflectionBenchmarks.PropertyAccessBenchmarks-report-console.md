
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method             | args   | Mean         | Error      | StdDev     | Gen0   | Gen1   | Gen2   | Allocated |
|------------------- |------- |-------------:|-----------:|-----------:|-------:|-------:|-------:|----------:|
| LinqExpression_Get | class  |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| LinqExpression_Set | class  |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| DynamicMethod_Get  | class  |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| DynamicMethod_Set  | class  |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| PSProperty_Get     | class  |     9.160 ns |  0.1573 ns |  0.1472 ns |      - |      - |      - |         - |
| PSProperty_Set     | class  |    63.749 ns |  1.0223 ns |  0.9563 ns | 0.0153 |      - |      - |      32 B |
| PSReflector_Get    | class  | 1,435.995 ns | 24.9780 ns | 23.3644 ns | 0.2956 | 0.0038 | 0.0019 |     648 B |
| PSReflector_Set    | class  |    29.879 ns |  0.4593 ns |  0.4297 ns |      - |      - |      - |         - |
| Reflection_Get     | class  |    10.080 ns |  0.1889 ns |  0.1767 ns |      - |      - |      - |         - |
| Reflection_Set     | class  |    18.702 ns |  0.3103 ns |  0.2903 ns |      - |      - |      - |         - |
| LinqExpression_Get | struct |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| LinqExpression_Set | struct |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| DynamicMethod_Get  | struct |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| DynamicMethod_Set  | struct |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| PSProperty_Get     | struct |    13.835 ns |  0.1961 ns |  0.1834 ns | 0.0115 |      - |      - |      24 B |
| PSProperty_Set     | struct |    52.432 ns |  1.0065 ns |  0.9414 ns | 0.0153 |      - |      - |      32 B |
| PSReflector_Get    | struct |    27.714 ns |  0.3966 ns |  0.3516 ns | 0.0114 |      - |      - |      24 B |
| PSReflector_Set    | struct |    26.984 ns |  0.4728 ns |  0.4423 ns |      - |      - |      - |         - |
| Reflection_Get     | struct |    14.896 ns |  0.2539 ns |  0.2251 ns | 0.0115 |      - |      - |      24 B |
| Reflection_Set     | struct |    16.976 ns |  0.1419 ns |  0.1328 ns |      - |      - |      - |         - |

Benchmarks with issues:
  PropertyAccessBenchmarks.LinqExpression_Get: DefaultJob [args=class]
  PropertyAccessBenchmarks.LinqExpression_Set: DefaultJob [args=class]
  PropertyAccessBenchmarks.DynamicMethod_Get: DefaultJob [args=class]
  PropertyAccessBenchmarks.DynamicMethod_Set: DefaultJob [args=class]
  PropertyAccessBenchmarks.LinqExpression_Get: DefaultJob [args=struct]
  PropertyAccessBenchmarks.LinqExpression_Set: DefaultJob [args=struct]
  PropertyAccessBenchmarks.DynamicMethod_Get: DefaultJob [args=struct]
  PropertyAccessBenchmarks.DynamicMethod_Set: DefaultJob [args=struct]
