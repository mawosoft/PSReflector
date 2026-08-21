// Copyright (c) Matthias Wolf, Mawosoft.

using System.Management.Automation;

namespace InvocationTestFixture.Internal;

internal static class SMA
{
    public static class Types
    {
        private const string _sma = "System.Management.Automation.";

        public const string DotNetAdapter = _sma + nameof(DotNetAdapter);
        public const string ExecutionContext = _sma + nameof(ExecutionContext);
        public const string MethodInformation = _sma + nameof(MethodInformation);

    }

#pragma warning disable CA1802 // Use literals where appropriate (false positive)
    public static class Members
    {
        // DotNetAdapter
        public const string GetMethodInformationArray = nameof(GetMethodInformationArray);
        // ExecutionContext
        public const string DollarErrorVariable = nameof(DollarErrorVariable);
        // MethodInformation
        public static readonly string _useReflection = nameof(_useReflection);
        // PSCodeMethod
        public static readonly string _codeReferenceMethodInformation = nameof(_codeReferenceMethodInformation);
        // Runspace et al.
        public const string ExecutionContext = nameof(ExecutionContext);

        static Members()
        {
            if (typeof(PSObject).Assembly.GetName().Version.Major < 6)
            {
                _useReflection = _useReflection.Substring(1);
                _codeReferenceMethodInformation = _codeReferenceMethodInformation.Substring(1);
            }
        }
    }
#pragma warning restore CA1802 // Use literals where appropriate

    extension(PSObject)
    {
        /// <summary>
        /// Similar to the internal <c>PSObject.Base()</c>, but also handles PSReference and keeps
        /// AutomationNull.
        /// </summary>
        public static object? Unwrap(object? obj)
        {
            if (obj is PSReference psr) obj = psr.Value;
            while (obj is PSObject pso)
            {
                var baseObj = pso.ImmediateBaseObject;
                // This breaks for AutomationNull.Value as well because that is just a specific
                // empty instance of PSObject.
                if (baseObj is PSCustomObject) break;
                obj = baseObj;
            }
            return obj;
        }
    }
}
