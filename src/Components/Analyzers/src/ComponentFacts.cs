// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Microsoft.AspNetCore.Components.Analyzers
{
    internal static class ComponentFacts
    {
        public static bool IsAnyParameter(ComponentSymbols symbols, IPropertySymbol property)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return property.GetAttributes().Any(a =>
            {
                return a.AttributeClass == symbols.ParameterAttribute || a.AttributeClass == symbols.CascadingParameterAttribute;
            });
        }

        public static bool IsParameter(ComponentSymbols symbols, IPropertySymbol property)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var propertyAttributes = property.GetAttributes();
            var hasParameterAttribute = HasParameterAttribute(symbols, propertyAttributes);
            return hasParameterAttribute;
        }

        public static bool HasParameterAttribute(ComponentSymbols symbols, ImmutableArray<AttributeData> attributes)
        {
            for (var i = 0; i < attributes.Length; i++)
            {
                if (attributes[i].AttributeClass == symbols.ParameterAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsParameterWithCaptureUnmatchedValues(ComponentSymbols symbols, IPropertySymbol property)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var attribute = property.GetAttributes().FirstOrDefault(a => a.AttributeClass == symbols.ParameterAttribute);
            if (attribute == null)
            {
                return false;
            }

            foreach (var kvp in attribute.NamedArguments)
            {
                if (string.Equals(kvp.Key, ComponentsApi.ParameterAttribute.CaptureUnmatchedValues, StringComparison.Ordinal))
                {
                    return kvp.Value.Value as bool? ?? false;
                }
            }

            return false;
        }

        public static bool IsCascadingParameter(ComponentSymbols symbols, IPropertySymbol property)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return property.GetAttributes().Any(a => a.AttributeClass == symbols.CascadingParameterAttribute);
        }

        public static bool IsComponent(ComponentSymbols symbols, INamedTypeSymbol type)
        {
            if (symbols is null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!IsAssignableFrom(symbols.IComponentType, type))
            {
                return false;
            }

            return true;
        }

        private static bool IsAssignableFrom(ITypeSymbol source, ITypeSymbol target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source == target)
            {
                return true;
            }

            if (source.TypeKind == TypeKind.Interface)
            {
                foreach (var @interface in target.AllInterfaces)
                {
                    if (source == @interface)
                    {
                        return true;
                    }
                }

                return false;
            }

            foreach (var type in target.GetTypeHierarchy())
            {
                if (source == type)
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<ITypeSymbol> GetTypeHierarchy(this ITypeSymbol typeSymbol)
        {
            while (typeSymbol != null)
            {
                yield return typeSymbol;

                typeSymbol = typeSymbol.BaseType;
            }
        }
    }
}
