
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method             | args   | Mean         | Error      | StdDev     | Gen0   | Gen1   | Gen2   | Allocated |
|------------------- |------- |-------------:|-----------:|-----------:|-------:|-------:|-------:|----------:|
| LinqExpression_Get | class  |     1.932 ns |  0.0890 ns |  0.0789 ns |      - |      - |      - |         - |
| LinqExpression_Set | class  |     4.070 ns |  0.0835 ns |  0.0781 ns |      - |      - |      - |         - |
| DynamicMethod_Get  | class  |     2.460 ns |  0.1041 ns |  0.1317 ns |      - |      - |      - |         - |
| DynamicMethod_Set  | class  |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| PSProperty_Get     | class  |     9.141 ns |  0.1041 ns |  0.0974 ns |      - |      - |      - |         - |
| PSProperty_Set     | class  |    62.432 ns |  0.8665 ns |  0.8105 ns | 0.0153 |      - |      - |      32 B |
| PSReflector_Get    | class  | 1,398.655 ns | 27.8105 ns | 26.0140 ns | 0.2956 | 0.0038 | 0.0019 |     648 B |
| PSReflector_Set    | class  |    21.652 ns |  0.3188 ns |  0.2982 ns |      - |      - |      - |         - |
| Reflection_Get     | class  |     3.971 ns |  0.0826 ns |  0.0773 ns |      - |      - |      - |         - |
| Reflection_Set     | class  |    11.755 ns |  0.2623 ns |  0.2576 ns |      - |      - |      - |         - |
| LinqExpression_Get | struct |     7.248 ns |  0.1969 ns |  0.1934 ns | 0.0115 |      - |      - |      24 B |
| LinqExpression_Set | struct |     1.380 ns |  0.0418 ns |  0.0391 ns |      - |      - |      - |         - |
| DynamicMethod_Get  | struct |     7.235 ns |  0.1749 ns |  0.1636 ns | 0.0115 |      - |      - |      24 B |
| DynamicMethod_Set  | struct |           NA |         NA |         NA |     NA |     NA |     NA |        NA |
| PSProperty_Get     | struct |    14.221 ns |  0.2834 ns |  0.2651 ns | 0.0115 |      - |      - |      24 B |
| PSProperty_Set     | struct |    52.867 ns |  1.0554 ns |  1.3723 ns | 0.0153 |      - |      - |      32 B |
| PSReflector_Get    | struct |    22.756 ns |  0.3087 ns |  0.2887 ns | 0.0115 |      - |      - |      24 B |
| PSReflector_Set    | struct |    16.523 ns |  0.2163 ns |  0.2023 ns |      - |      - |      - |         - |
| Reflection_Get     | struct |    14.834 ns |  0.1391 ns |  0.1086 ns | 0.0115 |      - |      - |      24 B |
| Reflection_Set     | struct |     9.809 ns |  0.1102 ns |  0.0977 ns |      - |      - |      - |         - |

Benchmarks with issues:
  FieldAccessBenchmarks.DynamicMethod_Set: DefaultJob [args=class]
  FieldAccessBenchmarks.DynamicMethod_Set: DefaultJob [args=struct]
