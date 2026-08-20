// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using InvocationTestFixture.Internal;

namespace InvocationTestFixture;

public sealed partial class InvocationTransformer : AstVisitor2
{
    private const string ReturnVariableText = "$_retval";
    private const string ArgsVariableText = "$_arguments";
    private const string NullVariableText = "$null";

    private static readonly ReadOnlyCollection<ExpressionAst> s_expressionsEmpty = new([]);

    private readonly Ast _ast;
    private readonly bool _useReflectionInvoke;
    private readonly string _instanceName;
    private readonly StringBuilder _resultBuilder = new(1024);
    private readonly string _astText;
    private int _astOffset;

    public static string Transform(Ast ast, string instanceName = "methodProvider")
    {
        ArgumentNullException.ThrowIfNull(ast);
        ArgumentException.ThrowIfNullOrEmpty(instanceName);
        var transformer = new InvocationTransformer(ast, false, instanceName);
        ast.Visit(transformer);
        return transformer.CompleteResult();
    }

    public static string TransformWithReflection(Ast ast, string instanceName = "methodProvider")
    {
        var transformer = new InvocationTransformer(ast, true, instanceName);
        ast.Visit(transformer);
        return transformer.CompleteResult();
    }

    private InvocationTransformer(Ast ast, bool useReflectionInvoke, string instanceName)
    {
        ArgumentNullException.ThrowIfNull(ast);
        ArgumentException.ThrowIfNullOrEmpty(instanceName);
        _ast = ast;
        _useReflectionInvoke = useReflectionInvoke;
        _instanceName = instanceName;
        _astText = _ast.Extent.Text;
        _astOffset = ast.Extent.StartOffset;
    }

    private string CompleteResult()
    {
        _resultBuilder.Append(_astText, _astOffset - _ast.Extent.StartOffset, _ast.Extent.EndOffset - _astOffset);
        _astOffset = _ast.Extent.EndOffset;
        return _resultBuilder.ToString();
    }

    private static string GetArrayPrefix(int length) => length == 1 ? "@(, " : "@(";

    public override AstVisitAction VisitInvokeMemberExpression([NotNull] InvokeMemberExpressionAst methodCallAst)
    {
        if (methodCallAst.Static
            || methodCallAst.Expression is not VariableExpressionAst ve
            || !string.Equals(ve.VariablePath.UserPath, _instanceName, StringComparison.OrdinalIgnoreCase)
            || methodCallAst.Member is not StringConstantExpressionAst sce)
        {
            return AstVisitAction.Continue;
        }

        string instanceText = ve.Extent.Text;
        string methodName = sce.Value;
        string argsText = "@()";
        int argsEndOffset = 0;
        ReadOnlyCollection<ExpressionAst> args = methodCallAst.Arguments ?? s_expressionsEmpty;
        if (args.Count != 0)
        {
            argsEndOffset = args[args.Count - 1].Extent.EndOffset - _ast.Extent.StartOffset;
            int start = args[0].Extent.StartOffset - _ast.Extent.StartOffset;
            argsText = GetArrayPrefix(args.Count) + _astText.Substring(start, argsEndOffset - start) + ")";
        }
        _resultBuilder.Append(_astText, _astOffset - _ast.Extent.StartOffset, methodCallAst.Extent.StartOffset - _astOffset);
        _astOffset = methodCallAst.Extent.EndOffset;
        var sb = new ScriptBuilder(_resultBuilder).WithIndentSize(methodCallAst.Extent.StartColumnNumber - 1);
        sb.BeginInvocation(instanceText, methodName, argsText);

        if (!_useReflectionInvoke)
        {
            sb.SimpleInvoke(methodCallAst).EndInvocation(instanceText, argsText);
            return AstVisitAction.SkipChildren;
        }

        var methodInfo = new StrictType(typeof(DefaultMethodProvider)).GetMethod(methodName);
        var paramInfos = methodInfo.GetParameters();
        if (!paramInfos.Any(pi => pi.ParameterType.IsByRef || pi.ParameterType.IsArray))
        {
            sb.ReflectionInvokePartial(instanceText, methodName).Append(argsText).AppendLine(")")
              .EndInvocation(instanceText, argsText);
            return AstVisitAction.SkipChildren;
        }

        // Handle each argument separately
        sb.Indent().Append(ArgsVariableText).Append(" = ").Append(GetArrayPrefix(paramInfos.Length));
        bool comma = false;
        bool hasByRef = false;
        bool hasParamArray = false;
        int minCount = paramInfos.Length;
        if (paramInfos.HasParamArray)
        {
            hasParamArray = true;
            minCount--;
        }
        for (int i = 0; i < minCount; i++)
        {
            if (comma) sb.Append(", ");
            comma = true;
            ExpressionAst? arg = i < args.Count ? args[i] : null;
            Type elementType = paramInfos[i].ParameterType;
            if (elementType.IsByRef)
            {
                hasByRef = true;
                elementType = elementType.GetElementType();
                if (arg is ConvertExpressionAst ce) arg = ce.Child;
            }
            if (elementType.IsArray)
            {
                elementType = elementType.GetElementType();
                sb.ArrayCast(elementType);
            }
            sb.Append(arg is null ? NullVariableText : arg.Extent.Text);
        }
        if (hasParamArray)
        {
            if (comma) sb.Append(", ");
            Type elementType = paramInfos[minCount].ParameterType.GetElementType();
            sb.ArrayCast(elementType);
            if (args.Count <= minCount)
            {
                sb.Append("@()");
            }
            else if (args.Count == minCount + 1)
            {
                ExpressionAst arg = args[minCount];
                string argText = arg.Extent.Text;
                if (string.Equals(argText, NullVariableText, StringComparison.OrdinalIgnoreCase)
                    || arg.Find(a => a is ArrayExpressionAst or ArrayLiteralAst, true) is not null)
                {
                    sb.Append(argText);
                }
                else
                {
                    sb.Append("@(, ").Append(argText).Append(')');
                }
            }
            else
            {
                int start = args[minCount].Extent.StartOffset - _ast.Extent.StartOffset;
                sb.Append("@(").Append(_astText, start, argsEndOffset - start).Append(')');
            }
        }
        sb.AppendLine(")")
          .ReflectionInvokePartial(instanceText, methodName).Append(ArgsVariableText).AppendLine(")")
          .EndInvocation(instanceText, hasByRef ? ArgsVariableText : argsText);
        return AstVisitAction.SkipChildren;
    }
}
