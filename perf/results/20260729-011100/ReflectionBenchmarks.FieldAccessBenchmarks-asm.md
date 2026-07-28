## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.FieldAccessBenchmarks.Direct_Get(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.FieldInfo,ReflectionBenchmarks.TestClass,System.Action>)
       push      rbx
       sub       rsp,20
       mov       ecx,[rdx+18]
       cmp       ecx,1
       je        short M00_L02
       cmp       ecx,2
       jne       short M00_L01
       mov       rcx,[rdx+20]
       cmp       qword ptr [rcx+8],0
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
       mov       rdx,7FFBC5098420
       call      qword ptr [7FFBC4E5F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51BFE40]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rax,[rdx+20]
       cmp       dword ptr [rax+18],0
       sete      al
       movzx     eax,al
       jmp       short M00_L00
; Total bytes of code 115
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.FieldAccessBenchmarks.Direct_Set(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.FieldInfo,ReflectionBenchmarks.TestClass,System.Action>)
       push      rbx
       sub       rsp,20
       mov       [rsp+30],rcx
       mov       eax,[rdx+18]
       cmp       eax,1
       je        short M00_L02
       cmp       eax,2
       jne       short M00_L01
       mov       rdx,[rdx+20]
       lea       rcx,[rdx+8]
       mov       rdx,[rsp+30]
       mov       rdx,[rdx+8]
       call      CORINFO_HELP_ASSIGN_REF
M00_L00:
       nop
       add       rsp,20
       pop       rbx
       ret
M00_L01:
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5098420
       call      qword ptr [7FFBC4E5F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51BFE10]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rax,[rdx+20]
       mov       dword ptr [rax+18],0DE
       jmp       short M00_L00
; Total bytes of code 125
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.FieldAccessBenchmarks.Direct_Get(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.FieldInfo,ReflectionBenchmarks.TestClass,System.Action>)
       push      rbx
       sub       rsp,20
       mov       eax,[rdx+18]
       cmp       eax,1
       jne       short M00_L01
       mov       rax,[rdx+20]
       cmp       dword ptr [rax+18],0
       sete      al
       movzx     eax,al
M00_L00:
       add       rsp,20
       pop       rbx
       ret
M00_L01:
       cmp       eax,2
       je        short M00_L02
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC50A8420
       call      qword ptr [7FFBC4E6F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51CFE40]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rax,[rdx+20]
       cmp       qword ptr [rax+8],0
       sete      al
       movzx     eax,al
       jmp       short M00_L00
; Total bytes of code 115
```

## .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3 (Job: DefaultJob)

```assembly
; ReflectionBenchmarks.FieldAccessBenchmarks.Direct_Set(ReflectionBenchmarks.TestClassMemberAccess`3<System.Reflection.FieldInfo,ReflectionBenchmarks.TestClass,System.Action>)
       push      rbx
       sub       rsp,20
       mov       [rsp+30],rcx
       mov       eax,[rdx+18]
       cmp       eax,1
       jne       short M00_L01
       mov       rax,[rdx+20]
       mov       dword ptr [rax+18],0DE
M00_L00:
       add       rsp,20
       pop       rbx
       ret
M00_L01:
       cmp       eax,2
       je        short M00_L02
       mov       rcx,offset MT_System.ArgumentOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       ecx,3
       mov       rdx,7FFBC5098420
       call      qword ptr [7FFBC4E5F228]
       mov       rdx,rax
       mov       rcx,rbx
       call      qword ptr [7FFBC51BFDF8]
       mov       rcx,rbx
       call      CORINFO_HELP_THROW
       int       3
M00_L02:
       mov       rdx,[rdx+20]
       lea       rcx,[rdx+8]
       mov       rdx,[rsp+30]
       mov       rdx,[rdx+8]
       call      CORINFO_HELP_ASSIGN_REF
       jmp       short M00_L00
; Total bytes of code 124
```

