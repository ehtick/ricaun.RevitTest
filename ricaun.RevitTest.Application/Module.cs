using ricaun.RevitTest.Application.Extensions;
using ricaun.RevitTest.Application.Revit;
using System;
using System.Runtime.CompilerServices;

namespace ricaun.RevitTest.Application
{
    class Module
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            using (AppDomain.CurrentDomain.GetAssemblyResolveDisposable().AddDelegatesAfterDispose())
            {
                CosturaUtility.Initialize();
                TestUtils.Initialize();
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class ModuleInitializerAttribute : Attribute { }
}
