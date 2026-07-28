
BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7417/22H2/2022Update) (VirtualBox)
AMD Ryzen 5 3400G with Radeon Vega Graphics 3.69GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


| Method                   | args   | Mean     | Error     | StdDev    | Median   | Code Size | Allocated |
|------------------------- |------- |---------:|----------:|----------:|---------:|----------:|----------:|
| TypedDelegate_Get        | class  | 2.711 ns | 0.2118 ns | 0.5869 ns | 2.445 ns |     358 B |         - |
| TypedDelegate_Get_Unsafe | class  | 2.377 ns | 0.0447 ns | 0.0418 ns | 2.370 ns |     129 B |         - |
| TypedDelegate_Set        | class  | 4.012 ns | 0.0660 ns | 0.0618 ns | 4.024 ns |     356 B |         - |
| TypedDelegate_Set_Unsafe | class  | 3.725 ns | 0.0634 ns | 0.0593 ns | 3.723 ns |     129 B |         - |
| TypedDelegate_Get        | struct | 2.438 ns | 0.0655 ns | 0.0612 ns | 2.446 ns |     370 B |         - |
| TypedDelegate_Get_Unsafe | struct | 2.430 ns | 0.0524 ns | 0.0490 ns | 2.438 ns |     131 B |         - |
| TypedDelegate_Set        | struct | 1.942 ns | 0.0413 ns | 0.0367 ns | 1.939 ns |     352 B |         - |
| TypedDelegate_Set_Unsafe | struct | 1.901 ns | 0.0289 ns | 0.0271 ns | 1.902 ns |     126 B |         - |
