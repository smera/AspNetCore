// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Microsoft.AspNetCore.Components.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ComponentParameterUsageAnalyzer : DiagnosticAnalyzer
    {
        public ComponentParameterUsageAnalyzer()
        {
            SupportedDiagnostics = ImmutableArray.Create(new[]
            {
                DiagnosticDescriptors.ComponentParametersShouldNotBeReferencedOutsideOfComponents,
            });
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(context =>
            {
                if (!ComponentSymbols.TryCreate(context.Compilation, out var symbols))
                {
                    // Types we need are not defined.
                    return;
                }

                context.RegisterOperationBlockStartAction(blockContext =>
                {
                    var containingType = blockContext.OwningSymbol?.ContainingType;
                    if (containingType != null && ComponentFacts.IsComponent(symbols, blockContext.OwningSymbol.ContainingType))
                    {
                        // We only want to look at properties that are not in component classes.
                        return;
                    }

                    blockContext.RegisterOperationAction(context =>
                    {
                        var propertyReference = (IPropertyReferenceOperation)context.Operation;
                        var componentProperty = propertyReference.Member;
                        var propertyAttributes = componentProperty.GetAttributes();

                        if (!ComponentFacts.HasParameterAttribute(symbols, propertyAttributes))
                        {
                            // This is not a property reference that we care about, it is not decorated with [Parameter].
                            return;
                        }

                        var containingType = componentProperty.ContainingType;
                        if (!ComponentFacts.IsComponent(symbols, containingType))
                        {
                            // Someone referenced a property as [Parameter] that is not an actual component.
                            return;
                        }

                        // At this point the user is referencing a component parameter outside of a component class.

                        context.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.ComponentParametersShouldNotBeReferencedOutsideOfComponents,
                            propertyReference.Syntax.GetLocation(),
                            propertyReference.Member.Name));

                    }, OperationKind.PropertyReference);
                });
            });
        }
    }
}
