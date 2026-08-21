// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using InvocationTestFixture.Internal;

namespace InvocationTestFixture;

public sealed class InvocationTransformer : AstVisitor2
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
    private int _indentSize;

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

    private void AppendBeforeAst(Ast ast)
    {
        _resultBuilder.Append(_astText, _astOffset - _ast.Extent.StartOffset, ast.Extent.StartOffset - _astOffset);
        _astOffset = ast.Extent.EndOffset;
        _indentSize = ast.Extent.StartColumnNumber - 1;
    }

    private void AppendIndent()
    {
        if (_indentSize > 0) _resultBuilder.Append(' ', _indentSize);
    }

    private void AppendBeginInvocation(string instanceText, string methodName, string argsText)
    {
        _resultBuilder.Append(instanceText).Append('.').Append(nameof(MethodProviderBase.BeginInvocation))
            .Append("('").Append(methodName).Append("', ").Append(argsText).AppendLine(")");
    }

    private void AppendEndInvocation(string instanceText, string argsText)
    {
        AppendIndent();
        _resultBuilder.Append(instanceText).Append('.').Append(nameof(MethodProviderBase.EndInvocation))
            .Append($"({ReturnVariableText}, ").Append(argsText).Append(')');
    }

    private void AppendSimpleInvoke(InvokeMemberExpressionAst methodCallAst)
    {
        AppendIndent();
        _resultBuilder.Append($"{ReturnVariableText} = ").AppendLine(methodCallAst.Extent.Text);
    }

    private void AppendReflectionInvokePartial(string instanceText, string methodName)
    {
        AppendIndent();
        _resultBuilder.Append($"{ReturnVariableText} = ").Append(instanceText)
            .Append(".GetType().GetMethod('").Append(methodName).Append("').Invoke(")
            .Append(instanceText).Append(", ");
    }

    private void AppendArrayCast(Type type)
    {
        // TODO FullName or Name
        _resultBuilder.Append('[').Append(type.Name).Append("[]]");
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
        AppendBeforeAst(methodCallAst);
        AppendBeginInvocation(instanceText, methodName, argsText);

        if (!_useReflectionInvoke)
        {
            AppendSimpleInvoke(methodCallAst);
            AppendEndInvocation(instanceText, argsText);
            return AstVisitAction.SkipChildren;
        }

        var methodInfo = new StrictType(typeof(DefaultMethodProvider)).GetMethod(methodName);
        var paramInfos = methodInfo.GetParameters();
        if (!paramInfos.Any(pi => pi.ParameterType.IsByRef || pi.ParameterType.IsArray))
        {
            AppendReflectionInvokePartial(instanceText, methodName);
            _resultBuilder.Append(argsText).AppendLine(")");
            AppendEndInvocation(instanceText, argsText);
            return AstVisitAction.SkipChildren;
        }

        // Handle each argument separately
        AppendIndent();
        _resultBuilder.Append(ArgsVariableText).Append(" = ").Append(GetArrayPrefix(paramInfos.Length));
        bool comma = false;
        bool hasByRef = false;
        bool hasParamArray = false;
        int minCount = paramInfos.Length;
        if (paramInfos[minCount - 1].IsDefined(typeof(ParamArrayAttribute), inherit: false))
        {
            hasParamArray = true;
            minCount--;
        }
        for (int i = 0; i < minCount; i++)
        {
            if (comma) _resultBuilder.Append(", ");
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
                AppendArrayCast(elementType);
            }
            _resultBuilder.Append(arg is null ? NullVariableText : arg.Extent.Text);
        }
        if (hasParamArray)
        {
            if (comma) _resultBuilder.Append(", ");
            Type elementType = paramInfos[minCount].ParameterType.GetElementType();
            AppendArrayCast(elementType);
            if (args.Count <= minCount)
            {
                _resultBuilder.Append("@()");
            }
            else if (args.Count == minCount + 1)
            {
                ExpressionAst arg = args[minCount];
                string argText = arg.Extent.Text;
                if (string.Equals(argText, NullVariableText, StringComparison.OrdinalIgnoreCase)
                    || arg.Find(a => a is ArrayExpressionAst or ArrayLiteralAst, true) is not null)
                {
                    _resultBuilder.Append(argText);
                }
                else
                {
                    _resultBuilder.Append("@(, ").Append(argText).Append(')');
                }
            }
            else
            {
                int start = args[minCount].Extent.StartOffset - _ast.Extent.StartOffset;
                _resultBuilder.Append("@(").Append(_astText, start, argsEndOffset - start).Append(')');
            }
        }
        _resultBuilder.AppendLine(")");
        AppendReflectionInvokePartial(instanceText, methodName);
        _resultBuilder.Append(ArgsVariableText).AppendLine(")");
        AppendEndInvocation(instanceText, hasByRef ? ArgsVariableText : argsText);
        return AstVisitAction.SkipChildren;
    }
}
