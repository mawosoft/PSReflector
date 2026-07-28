## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Get(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,rdx
       mov       edx,[rbx+18]
       cmp       edx,1
       je        short M00_L03
       cmp       edx,2
       jne       short M00_L02
       mov       rdx,[rbx+28]
       mov       rsi,rdx
       test      rsi,rsi
       je        short M00_L00
       mov       rcx,offset MT_System.Func<ReflectionBenchmarks.TestClass, System.Version>
       cmp       [rsi],rcx
       jne       near ptr M00_L04
M00_L00:
       mov       rdx,[rbx+20]
       mov       rcx,[rsi+8]
       call      qword ptr [rsi+18]
       test      rax,rax
       sete      al
       movzx     eax,al
M00_L01:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L02:
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5098420
       call      qword ptr [7FFBC4E5F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51BFDC8]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L03:
       mov       rdx,[rbx+28]
       mov       rcx,offset MT_System.Func<ReflectionBenchmarks.TestClass, System.Int32>
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       mov       rdx,[rbx+20]
       mov       rcx,[rax+8]
       call      qword ptr [rax+18]
       test      eax,eax
       sete      al
       movzx     eax,al
       jmp       short M00_L01
M00_L04:
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       mov       rsi,rax
       jmp       near ptr M00_L00
; Total bytes of code 193
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       push      rsi
       push      rbx
       test      rdx,rdx
       je        short M01_L00
       mov       rax,[rdx]
       cmp       rax,rcx
       jne       short M01_L01
M01_L00:
       mov       rax,rdx
       pop       rbx
       pop       rsi
       ret
M01_L01:
       mov       r8,2101C000038
       mov       r8,[r8]
       add       r8,10
       rorx      r10,rax,20
       xor       r10,rcx
       mov       r9,9E3779B97F4A7C15
       imul      r10,r9
       mov       r9d,[r8]
       shrx      r10,r10,r9
       xor       r9d,r9d
       cmp       r9d,8
       jge       short M01_L05
M01_L02:
       lea       r11d,[r10+1]
       movsxd    r11,r11d
       lea       r11,[r11+r11*2]
       lea       r11,[r8+r11*8]
       mov       ebx,[r11]
       mov       rsi,[r11+8]
       and       ebx,0FFFFFFFE
       cmp       rsi,rax
       jne       short M01_L03
       mov       rsi,rcx
       xor       rsi,[r11+10]
       cmp       rsi,1
       jbe       short M01_L04
M01_L03:
       test      ebx,ebx
       je        short M01_L05
       inc       r9d
       add       r10d,r9d
       and       r10d,[r8+4]
       cmp       r9d,8
       jl        short M01_L02
       jmp       short M01_L05
M01_L04:
       cmp       ebx,[r11]
       jne       short M01_L05
       cmp       esi,1
       je        near ptr M01_L00
M01_L05:
       pop       rbx
       pop       rsi
       jmp       qword ptr [7FFBC4E56340]; System.Runtime.CompilerServices.CastHelpers.ChkCastAny_NoCacheLookup(Void*, System.Object)
; Total bytes of code 165
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Get_Unsafe(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rbx
       sub       rsp,20
       mov       rbx,[rdx+28]
       mov       ecx,[rdx+18]
       cmp       ecx,1
       je        short M00_L02
       cmp       ecx,2
       jne       short M00_L01
       mov       rdx,[rdx+20]
       mov       rcx,[rbx+8]
       call      qword ptr [rbx+18]
       test      rax,rax
       sete      al
       movzx     eax,al
M00_L00:
       add       rsp,20
       pop       rbx
       ret
M00_L01:
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5088420
       call      qword ptr [7FFBC4E4F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51AFEA0]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rdx,[rdx+20]
       mov       rcx,[rbx+8]
       call      qword ptr [rbx+18]
       test      eax,eax
       sete      al
       movzx     eax,al
       jmp       short M00_L00
; Total bytes of code 129
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Set(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rsi,rcx
       mov       rbx,rdx
       mov       edx,[rbx+18]
       cmp       edx,1
       je        short M00_L02
       cmp       edx,2
       jne       short M00_L01
       mov       rdx,[rbx+28]
       mov       rax,rdx
       test      rax,rax
       je        short M00_L00
       mov       rcx,offset MT_System.Action<ReflectionBenchmarks.TestClass, System.Version>
       cmp       [rax],rcx
       jne       near ptr M00_L03
M00_L00:
       mov       rdx,[rbx+20]
       mov       r8,[rsi+8]
       mov       rcx,[rax+8]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax+18]
M00_L01:
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5088420
       call      qword ptr [7FFBC4E4F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51AFE40]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rdx,[rbx+28]
       mov       rcx,offset MT_System.Action<ReflectionBenchmarks.TestClass, System.Int32>
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       mov       rdx,[rbx+20]
       mov       r8d,0DE
       mov       rcx,[rax+8]
       add       rsp,28
       pop       rbx
       pop       rsi
       jmp       qword ptr [rax+18]
M00_L03:
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       jmp       near ptr M00_L00
; Total bytes of code 191
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       push      rsi
       push      rbx
       test      rdx,rdx
       je        short M01_L00
       mov       rax,[rdx]
       cmp       rax,rcx
       jne       short M01_L01
M01_L00:
       mov       rax,rdx
       pop       rbx
       pop       rsi
       ret
M01_L01:
       mov       r8,21F6B800038
       mov       r8,[r8]
       add       r8,10
       rorx      r10,rax,20
       xor       r10,rcx
       mov       r9,9E3779B97F4A7C15
       imul      r10,r9
       mov       r9d,[r8]
       shrx      r10,r10,r9
       xor       r9d,r9d
       cmp       r9d,8
       jge       short M01_L05
M01_L02:
       lea       r11d,[r10+1]
       movsxd    r11,r11d
       lea       r11,[r11+r11*2]
       lea       r11,[r8+r11*8]
       mov       ebx,[r11]
       mov       rsi,[r11+8]
       and       ebx,0FFFFFFFE
       cmp       rsi,rax
       jne       short M01_L03
       mov       rsi,rcx
       xor       rsi,[r11+10]
       cmp       rsi,1
       jbe       short M01_L04
M01_L03:
       test      ebx,ebx
       je        short M01_L05
       inc       r9d
       add       r10d,r9d
       and       r10d,[r8+4]
       cmp       r9d,8
       jl        short M01_L02
       jmp       short M01_L05
M01_L04:
       cmp       ebx,[r11]
       jne       short M01_L05
       cmp       esi,1
       je        near ptr M01_L00
M01_L05:
       pop       rbx
       pop       rsi
       jmp       qword ptr [7FFBC4E46340]; System.Runtime.CompilerServices.CastHelpers.ChkCastAny_NoCacheLookup(Void*, System.Object)
; Total bytes of code 165
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Set_Unsafe(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rbx
       sub       rsp,20
       mov       rax,[rdx+28]
       mov       r8d,[rdx+18]
       cmp       r8d,1
       je        short M00_L01
       cmp       r8d,2
       jne       short M00_L00
       mov       r8,[rcx+8]
       mov       rdx,[rdx+20]
       mov       rcx,[rax+8]
       add       rsp,20
       pop       rbx
       jmp       qword ptr [rax+18]
M00_L00:
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5098420
       call      qword ptr [7FFBC4E5F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51BFE40]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L01:
       mov       rdx,[rdx+20]
       mov       r8d,0DE
       mov       rcx,[rax+8]
       add       rsp,20
       pop       rbx
       jmp       qword ptr [rax+18]
; Total bytes of code 129
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Get(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,rdx
       mov       esi,[rbx+18]
       cmp       esi,1
       jne       short M00_L02
       mov       rdx,[rbx+28]
       mov       rsi,rdx
       test      rsi,rsi
       je        short M00_L00
       mov       rcx,offset MT_System.Func<ReflectionBenchmarks.TestClass, System.Int32>
       cmp       [rsi],rcx
       jne       short M00_L03
M00_L00:
       mov       rdx,[rbx+20]
       mov       rcx,[rsi+8]
       call      qword ptr [rsi+18]
       test      eax,eax
       sete      al
       movzx     eax,al
M00_L01:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L02:
       cmp       esi,2
       je        short M00_L04
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5078420
       call      qword ptr [7FFBC4E3F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC519FE88]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L03:
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       mov       rsi,rax
       jmp       short M00_L00
M00_L04:
       mov       rdx,[rbx+28]
       mov       rsi,rdx
       test      rsi,rsi
       je        short M00_L05
       mov       rcx,offset MT_System.Func<ReflectionBenchmarks.TestClass, System.Version>
       cmp       [rsi],rcx
       je        short M00_L05
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       mov       rsi,rax
M00_L05:
       mov       rdx,[rbx+20]
       mov       rcx,[rsi+8]
       call      qword ptr [rsi+18]
       test      rax,rax
       sete      al
       movzx     eax,al
       jmp       near ptr M00_L01
; Total bytes of code 205
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       push      rsi
       push      rbx
       test      rdx,rdx
       je        short M01_L00
       mov       rax,[rdx]
       cmp       rax,rcx
       jne       short M01_L01
M01_L00:
       mov       rax,rdx
       pop       rbx
       pop       rsi
       ret
M01_L01:
       mov       r8,23231400038
       mov       r8,[r8]
       add       r8,10
       rorx      r10,rax,20
       xor       r10,rcx
       mov       r9,9E3779B97F4A7C15
       imul      r10,r9
       mov       r9d,[r8]
       shrx      r10,r10,r9
       xor       r9d,r9d
       cmp       r9d,8
       jge       short M01_L05
M01_L02:
       lea       r11d,[r10+1]
       movsxd    r11,r11d
       lea       r11,[r11+r11*2]
       lea       r11,[r8+r11*8]
       mov       ebx,[r11]
       mov       rsi,[r11+8]
       and       ebx,0FFFFFFFE
       cmp       rsi,rax
       jne       short M01_L03
       mov       rsi,rcx
       xor       rsi,[r11+10]
       cmp       rsi,1
       jbe       short M01_L04
M01_L03:
       test      ebx,ebx
       je        short M01_L05
       inc       r9d
       add       r10d,r9d
       and       r10d,[r8+4]
       cmp       r9d,8
       jl        short M01_L02
       jmp       short M01_L05
M01_L04:
       cmp       ebx,[r11]
       jne       short M01_L05
       cmp       esi,1
       je        near ptr M01_L00
M01_L05:
       pop       rbx
       pop       rsi
       jmp       qword ptr [7FFBC4E36340]; System.Runtime.CompilerServices.CastHelpers.ChkCastAny_NoCacheLookup(Void*, System.Object)
; Total bytes of code 165
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Get_Unsafe(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rsi
       push      rbx
       sub       rsp,28
       mov       rbx,[rdx+28]
       mov       esi,[rdx+18]
       cmp       esi,1
       jne       short M00_L01
       mov       rdx,[rdx+20]
       mov       rcx,[rbx+8]
       call      qword ptr [rbx+18]
       test      eax,eax
       sete      al
       movzx     eax,al
M00_L00:
       add       rsp,28
       pop       rbx
       pop       rsi
       ret
M00_L01:
       cmp       esi,2
       je        short M00_L02
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5078420
       call      qword ptr [7FFBC4E3F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC519FE88]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rdx,[rdx+20]
       mov       rcx,[rbx+8]
       call      qword ptr [rbx+18]
       test      rax,rax
       sete      al
       movzx     eax,al
       jmp       short M00_L00
; Total bytes of code 131
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Set(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rsi,rcx
       mov       rbx,rdx
       mov       edi,[rbx+18]
       cmp       edi,1
       jne       short M00_L01
       mov       rdx,[rbx+28]
       mov       rax,rdx
       test      rax,rax
       je        short M00_L00
       mov       rcx,offset MT_System.Action<ReflectionBenchmarks.TestClass, System.Int32>
       cmp       [rax],rcx
       jne       short M00_L02
M00_L00:
       mov       rdx,[rbx+20]
       mov       r8d,0DE
       mov       rcx,[rax+8]
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax+18]
M00_L01:
       cmp       edi,2
       je        short M00_L03
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5088420
       call      qword ptr [7FFBC4E4F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51AFD68]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       jmp       short M00_L00
M00_L03:
       mov       rdx,[rbx+28]
       mov       rcx,offset MT_System.Action<ReflectionBenchmarks.TestClass, System.Version>
       call      System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       mov       rdx,[rbx+20]
       mov       r8,[rsi+8]
       mov       rcx,[rax+8]
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       jmp       qword ptr [rax+18]
; Total bytes of code 187
```
```assembly
; System.Runtime.CompilerServices.CastHelpers.ChkCastAny(Void*, System.Object)
       push      rsi
       push      rbx
       test      rdx,rdx
       je        short M01_L00
       mov       rax,[rdx]
       cmp       rax,rcx
       jne       short M01_L01
M01_L00:
       mov       rax,rdx
       pop       rbx
       pop       rsi
       ret
M01_L01:
       mov       r8,1E9AAC00038
       mov       r8,[r8]
       add       r8,10
       rorx      r10,rax,20
       xor       r10,rcx
       mov       r9,9E3779B97F4A7C15
       imul      r10,r9
       mov       r9d,[r8]
       shrx      r10,r10,r9
       xor       r9d,r9d
       cmp       r9d,8
       jge       short M01_L05
M01_L02:
       lea       r11d,[r10+1]
       movsxd    r11,r11d
       lea       r11,[r11+r11*2]
       lea       r11,[r8+r11*8]
       mov       ebx,[r11]
       mov       rsi,[r11+8]
       and       ebx,0FFFFFFFE
       cmp       rsi,rax
       jne       short M01_L03
       mov       rsi,rcx
       xor       rsi,[r11+10]
       cmp       rsi,1
       jbe       short M01_L04
M01_L03:
       test      ebx,ebx
       je        short M01_L05
       inc       r9d
       add       r10d,r9d
       and       r10d,[r8+4]
       cmp       r9d,8
       jl        short M01_L02
       jmp       short M01_L05
M01_L04:
       cmp       ebx,[r11]
       jne       short M01_L05
       cmp       esi,1
       je        near ptr M01_L00
M01_L05:
       pop       rbx
       pop       rsi
       jmp       qword ptr [7FFBC4E46340]; System.Runtime.CompilerServices.CastHelpers.ChkCastAny_NoCacheLookup(Void*, System.Object)
; Total bytes of code 165
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.PropertyAccessBenchmarks.TypedDelegate_Set_Unsafe(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.PropertyInfo,ReflectionBenchmarks.TestClass,System.Delegate>)
       push      rbx
       sub       rsp,20
       mov       rax,[rdx+28]
       mov       ebx,[rdx+18]
       cmp       ebx,1
       jne       short M00_L00
       mov       rdx,[rdx+20]
       mov       r8d,0DE
       mov       rcx,[rax+8]
       add       rsp,20
       pop       rbx
       jmp       qword ptr [rax+18]
M00_L00:
       cmp       ebx,2
       je        short M00_L01
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5098420
       call      qword ptr [7FFBC4E5F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51BFE40]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L01:
       mov       r8,[rcx+8]
       mov       rdx,[rdx+20]
       mov       rcx,[rax+8]
       add       rsp,20
       pop       rbx
       jmp       qword ptr [rax+18]
; Total bytes of code 126
```

