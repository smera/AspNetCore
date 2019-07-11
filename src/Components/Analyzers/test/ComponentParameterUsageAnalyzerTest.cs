// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Microsoft.AspNetCore.Components.Analyzers.Tests
{
    public class ComponentParameterUsageAnalyzerTest : DiagnosticVerifier
    {
        public ComponentParameterUsageAnalyzerTest()
        {
            ComponentTestSource = $@"
    namespace ConsoleApplication1
    {{
        using {typeof(ParameterAttribute).Namespace};
        class TestComponent : IComponent
        {{
            [Parameter] public string TestProperty {{ get; set; }}
        }}
    }}" + ComponentsTestDeclarations.Source;
        }

        private string ComponentTestSource { get; }

        [Fact]
        public void ComponentPropertyReferenceInAssignment()
        {
            var test = $@"
    namespace ConsoleApplication1
    {{
        using {typeof(ParameterAttribute).Namespace};
        class TypeName
        {{
            void Method()
            {{
                var testComponent = new TestComponent();
                var value = testComponent.TestProperty;
            }}
        }}
    }}" + ComponentTestSource;

            VerifyCSharpDiagnostic(test,
                new DiagnosticResult
                {
                    Id = DiagnosticDescriptors.ComponentParametersShouldNotBeReferencedOutsideOfComponents.Id,
                    Message = "Component parameter 'TestProperty' should not be referenced outside of its Razor component.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 29)
                    }
                });
        }

        [Fact]
        public void ComponentPropertyReferenceExpression()
        {
            var test = $@"
    namespace ConsoleApplication1
    {{
        using {typeof(ParameterAttribute).Namespace};
        class TypeName
        {{
            void Method()
            {{
                System.IO.Console.WriteLine(new TestComponent().TestProperty);
            }}
        }}
    }}" + ComponentTestSource;

            VerifyCSharpDiagnostic(test,
                new DiagnosticResult
                {
                    Id = DiagnosticDescriptors.ComponentParametersShouldNotBeReferencedOutsideOfComponents.Id,
                    Message = "Component parameter 'TestProperty' should not be referenced outside of its Razor component.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 45)
                    }
                });
        }

        [Fact]
        public void ComponentPropertyReferenceInStatement()
        {
            var test = $@"
    namespace ConsoleApplication1
    {{
        using {typeof(ParameterAttribute).Namespace};
        class TypeName
        {{
            void Method()
            {{
                var testComponent = new TestComponent();
                for (var i = 0; i < testComponent.TestProperty.Length; i++)
                {{
                }}
            }}
        }}
    }}" + ComponentTestSource;

            VerifyCSharpDiagnostic(test,
                new DiagnosticResult
                {
                    Id = DiagnosticDescriptors.ComponentParametersShouldNotBeReferencedOutsideOfComponents.Id,
                    Message = "Component parameter 'TestProperty' should not be referenced outside of its Razor component.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 37)
                    }
                });
        }

        [Fact]
        public void ComponentPropertyReferenceIgnoresShadowedPropertyReference()
        {
            var test = $@"
    namespace ConsoleApplication1
    {{
        using {typeof(ParameterAttribute).Namespace};
        class TypeName
        {{
            void Method()
            {{
                var testComponent = new InheritedComponent();
                var value = testComponent.TestProperty;
            }}
        }}

        class InheritedComponent : TestComponent
        {{
            public new string TestProperty {{ get; set; }}
        }}
    }}" + ComponentTestSource;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void ComponentPropertyReferenceIgnoresPropertyReferencesInsideComponent()
        {
            var test = $@"
    namespace ConsoleApplication1
    {{
        using {typeof(ParameterAttribute).Namespace};
        class TypeName : IComponent
        {{
            void Method()
            {{
                System.IO.Console.WriteLine(TestProperty);
            }}

            [Parameter] public string TestProperty {{ get; set; }}
        }}
    }}" + ComponentTestSource;

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new ComponentParameterUsageAnalyzer();
    }
}
