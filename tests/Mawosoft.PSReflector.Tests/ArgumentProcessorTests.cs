// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;
using Xunit;

namespace Mawosoft.PSReflector;

public partial class ArgumentProcessorTests
{
    private sealed class PreProcess_TheoryData : TheoryData<string, MethodBase, object?[], object?[]?>
    {
        public PreProcess_TheoryData()
        {
            string hint = "none";
            var method = new MockMethodBase([]);
            Add(hint, method, [], null);
            Add(hint, method, null!, []);

            hint = "passthru";
            method = new MockMethodBase(
            [
                new MockParameterInit { Name = "p1", ParameterType = typeof(int), },
                new MockParameterInit { Name = "p2", ParameterType = typeof(string), },
            ]);
            Add(hint, method, [111, "Foo"], null);
            Add(hint, method, [111, null], null);
            Add(hint, method, [null, "Foo"], null);

            hint = "convert";
            object?[] expected = [111, "222"];
            Add(hint, method, ["111", 222], expected);
#pragma warning disable IDE0004 // Remove Unnecessary Cast (differs per framework)
            Add(hint, method, [new PSObject("111"), new PSObject((object)222)], expected);
#pragma warning restore IDE0004 // Remove Unnecessary Cast
            Add(hint, method, [AutomationNull.Value, AutomationNull.Value], [0, ""]);

            hint = "optional";
            method = new MockMethodBase(
            [
                new MockParameterInit
                {
                    Name = "p1",
                    ParameterType = typeof(int),
                    Attributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                    DefaultValue = 42,
                },
            ]);
            Add(hint, method, [111], null);
            expected = [42];
            Add(hint, method, null!, expected);
            Add(hint, method, [], expected);
            method = new MockMethodBase(
            [
                new MockParameterInit { Name = "p1", ParameterType = typeof(int), },
                new MockParameterInit
                {
                    Name = "p2",
                    ParameterType = typeof(int),
                    Attributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                    DefaultValue = 42,
                },
                new MockParameterInit
                {
                    Name = "p3",
                    ParameterType = typeof(int),
                    Attributes = ParameterAttributes.Optional | ParameterAttributes.HasDefault,
                    DefaultValue = 24,
                },
            ]);
            Add(hint, method, [1], [1, 42, 24]);
            Add(hint, method, [1, 2], [1, 2, 24]);

            hint = "params";
            method = new MockMethodBase(
            [
                new MockParameterInit { Name = "pa", ParameterType = typeof(int[]), IsParamArray = true, },
            ]);
            int[]? pa = [];
            expected = [pa];
            Add(hint, method, null!, expected);
            object?[] prefix = [];
            while (true)
            {
                pa = null;
                Add(hint, method, [.. prefix, pa], null);
                pa = [];
                expected = [.. prefix, pa];
                Add(hint, method, [.. prefix, pa], null);
                Add(hint, method, [.. prefix], expected);
                Add(hint, method, [.. prefix, Array.Empty<string>()], expected);
                pa = [11];
                expected = [.. prefix, pa];
                Add(hint, method, [.. prefix, pa], null);
                Add(hint, method, [.. prefix, 11], expected);
                Add(hint, method, [.. prefix, "11"], expected);
#pragma warning disable CA1861 // Avoid constant arrays as arguments
                Add(hint, method, [.. prefix, new string[] { "11" }], expected);
#pragma warning restore CA1861 // Avoid constant arrays as arguments
                Add(hint, method, [.. prefix, new object[] { "11" }], expected);
                pa = [11, 12, 13];
                expected = [.. prefix, pa];
                Add(hint, method, [.. prefix, pa], null);
                Add(hint, method, [.. prefix, 11, 12, 13], expected);
                Add(hint, method, [.. prefix, 11, "12", 13], expected);
                Add(hint, method, [.. prefix, new object[] { 11, 12, "13" }], expected);
                if (prefix.Length != 0) break;
                prefix = [1];
                method = new MockMethodBase(
                [
                    new MockParameterInit { Name = "p1", ParameterType = typeof(int), },
                    new MockParameterInit { Name = "pa", ParameterType = typeof(int[]), IsParamArray = true, },
                ]);
            }
        }
    }

    [Theory]
    [ClassData(typeof(PreProcess_TheoryData))]
    public void PreProcess_Succeeds(string hint, MethodBase method, object?[] arguments, object?[]? expected)
    {
        _ = hint;
        var processor = new ArgumentProcessor(method);
        if (expected is null)
        {
            expected = (object?[])arguments.Clone();
            var processed = processor.PreProcess(arguments, out bool postProcess);
            Assert.False(postProcess);
            Assert.Same(arguments, processed);
            Assert.Equal((IEnumerable<object?>)expected, processed, ReferenceEquals);
        }
        else
        {
            var processed = processor.PreProcess(arguments, out bool postProcess);
            Assert.False(postProcess);
            Assert.NotSame(arguments, processed);
            Assert.Equal(expected, processed);
        }
    }

    [Fact]
    public void ByRef_Succeeds()
    {
        Type byrefType = typeof(int).MakeByRefType();
        var method = new MockMethodBase(
        [
            new MockParameterInit { Name = "p0", ParameterType = typeof(int), },
            new MockParameterInit { Name = "p1", ParameterType = byrefType, Attributes = ParameterAttributes.In, },
            new MockParameterInit { Name = "p2", ParameterType = byrefType, Attributes = ParameterAttributes.Out, },
            new MockParameterInit { Name = "p3", ParameterType = byrefType, },
            new MockParameterInit { Name = "p4", ParameterType = byrefType, },
        ]);
        var processor = new ArgumentProcessor(method);
        PSReference[] refs = [new(null), new(null), new(12), new(13), new(14)];
        object?[] arguments = [1, 11, refs[2], new PSObject(refs[3]), refs[4]];
        object?[] expectedPost = (object?[])arguments.Clone();
        object?[] expectedPre = [1, 11, 12, 13, 14];
        var processed = processor.PreProcess(arguments, out bool postProcess);
        Assert.True(postProcess);
        Assert.Equal(expectedPre, processed);
        processed = [10, 110, 120, 130, 140];
        expectedPost[1] = processed[1];
        processor.PostProcess(processed, arguments);
        Assert.Equal((IEnumerable<object?>)expectedPost, arguments, ReferenceEquals);
        Assert.All(refs, (r, i) => { if (i > 1) Assert.Equal(processed[i], r.Value); });
    }
}
