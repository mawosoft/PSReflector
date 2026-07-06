// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Mawosoft.PSReflector;

public class MemberDescriptorTests
{
    [Fact]
    public void Ctor_Defaults()
    {
        var md = new MemberDescriptor();
        Assert.Null(md.Name);
        Assert.Null(md.PSName);
        Assert.False(md.ParamCount.HasValue);
        Assert.Null(md.MemberInfo);
        Assert.False(md.CanWrite);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Foo")]
    public void Ctor_Name(string name)
    {
        var md = new MemberDescriptor(name);
        Assert.Equal(name, md.Name);
        Assert.Null(md.PSName);
        Assert.False(md.ParamCount.HasValue);
        Assert.Null(md.MemberInfo);
        Assert.False(md.CanWrite);
    }

    [Fact]
    public void Ctor_Name_Null()
    {
        string name = null!;
        var md = new MemberDescriptor(name);
        Assert.Equal(name, md.Name);
        Assert.Null(md.PSName);
        Assert.False(md.ParamCount.HasValue);
        Assert.Null(md.MemberInfo);
        Assert.False(md.CanWrite);
    }

    [Fact]
    public void Ctor_MemberInfo()
    {
        MemberInfo? mi = typeof(object).GetMethod(nameof(ToString));
        Assert.NotNull(mi);
        var md = new MemberDescriptor(mi);
        Assert.Equal(mi, md.MemberInfo);
        Assert.Null(md.Name);
        Assert.Null(md.PSName);
        Assert.False(md.ParamCount.HasValue);
        Assert.False(md.CanWrite);
    }

    [Fact]
    public void Ctor_MemberInfo_Null()
    {
        MemberInfo mi = null!;
        var md = new MemberDescriptor(mi);
        Assert.Equal(mi, md.MemberInfo);
        Assert.Null(md.Name);
        Assert.Null(md.PSName);
        Assert.False(md.ParamCount.HasValue);
        Assert.False(md.CanWrite);
    }

    public class PropertyCombinations : TheoryData<string[]>
    {
        public PropertyCombinations()
        {
            var properties = typeof(MemberDescriptor).GetProperties();
            var combinationCount = 1 << properties.Length;
            List<string> combination = [];
            for (int mask = 0; mask < combinationCount; mask++)
            {
                combination.Clear();
                for (int i = 0; i < properties.Length; i++)
                {
                    if ((mask & (1 << i)) != 0) combination.Add(properties[i].Name);
                }
                Add(combination.ToArray());
            }
        }
    }

    [Theory]
    [ClassData(typeof(PropertyCombinations))]
    public void ToString_Succeeds(string[] initializedProperties)
    {
        var md = new MemberDescriptor
        {
            Name = "Foo",
            PSName = "Bar",
            ParamCount = 1,
            MemberInfo = typeof(object).GetMethod(nameof(ToString)),
            CanWrite = true,
        };
        var properties = typeof(MemberDescriptor).GetProperties();
        foreach (var p in properties)
        {
            if (Array.IndexOf(initializedProperties, p.Name) < 0)
            {
                p.SetValue(md, null);
            }
        }
        var result = md.ToString();
        Assert.StartsWith(nameof(MemberDescriptor) + " {", result, StringComparison.Ordinal);
        Assert.EndsWith("}", result, StringComparison.Ordinal);
        var pattern = " " + nameof(MemberDescriptor.Name) + ": ";
#pragma warning disable CA1508 // Avoid dead conditional code (false positive)
        if (md.Name is not null || (md.Name is null && md.MemberInfo is null))
#pragma warning restore CA1508 // Avoid dead conditional code
        {
            Assert.Contains(pattern, result, StringComparison.Ordinal);
        }
        else
        {
            Assert.DoesNotContain(pattern, result, StringComparison.Ordinal);
        }
        foreach (var p in properties)
        {
            if (p.Name == nameof(MemberDescriptor.Name)) continue;
            pattern = " " + p.Name + ": ";
            if (Array.IndexOf(initializedProperties, p.Name) >= 0)
            {
                Assert.Contains(pattern, result, StringComparison.Ordinal);
            }
            else
            {
                Assert.DoesNotContain(pattern, result, StringComparison.Ordinal);
            }
        }
    }
}
