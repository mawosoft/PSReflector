// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation.Language;
using System.Text;

namespace InvocationTestFixture;

public sealed partial class InvocationTransformer
{
    private readonly struct ScriptBuilder
    {
        private readonly StringBuilder _sb;
        private readonly int _indent;

        public ScriptBuilder(StringBuilder sb)
        {
            _sb = sb;
            _indent = 0;
        }

        public ScriptBuilder(StringBuilder sb, int indent)
        {
            _sb = sb;
            _indent = indent;
        }

        public ScriptBuilder Append(char value)
        {
            _sb.Append(value);
            return this;
        }

        public ScriptBuilder Append(string? value)
        {
            _sb.Append(value);
            return this;
        }

        public ScriptBuilder Append(string? value, int startIndex, int count)
        {
            _sb.Append(value, startIndex, count);
            return this;
        }

        public ScriptBuilder AppendLine()
        {
            _sb.AppendLine();
            return this;
        }

        public ScriptBuilder AppendLine(string? value)
        {
            _sb.AppendLine(value);
            return this;
        }

        public ScriptBuilder WithIndent(int indent)
        {
            return new(_sb, indent);
        }

        public ScriptBuilder Indent()
        {
            if (_indent > 0) _sb.Append(' ', _indent);
            return this;
        }

        public ScriptBuilder ArrayCast(Type type)
        {
            // TODO FullName or Name
            _sb.Append('[').Append(type.Name).Append("[]]");
            return this;
        }

        public ScriptBuilder BeginInvocation(string instanceText, string methodName, string argsText)
        {
            _sb.Append(instanceText).Append('.').Append(nameof(MethodProviderBase.BeginInvocation))
               .Append("('").Append(methodName).Append("', ").Append(argsText).AppendLine(")");
            return this;
        }

        public ScriptBuilder EndInvocation(string instanceText, string argsText)
        {
            Indent();
            _sb.Append(instanceText).Append('.').Append(nameof(MethodProviderBase.EndInvocation))
               .Append($"({ReturnVariableText}, ").Append(argsText).Append(')');
            return this;
        }

        public ScriptBuilder SimpleInvoke(InvokeMemberExpressionAst methodCallAst)
        {
            Indent();
            _sb.Append($"{ReturnVariableText} = ").AppendLine(methodCallAst.Extent.Text);
            return this;
        }

        public ScriptBuilder ReflectionInvokePartial(string instanceText, string methodName)
        {
            _sb.Append($"{ReturnVariableText} = ").Append(instanceText)
               .Append(".GetType().GetMethod('").Append(methodName).Append("').Invoke(")
               .Append(instanceText).Append(", ");
            return this;
        }
    }
}
