﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.RegistrationByConvention
{
    /// <summary>
    /// Provides helper methods to retrieve classes from assemblies.
    /// </summary>
    public static partial class AllClasses
    {
        private static readonly string NetFrameworkProductName = GetNetFrameworkProductName();
        private static readonly string UnityProductName = GetUnityProductName();

        /// <summary>
        /// Returns all visible, non-abstract classes from <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>All visible, non-abstract classes found in the assemblies.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="assemblies"/> contains <see langword="null"/> elements.</exception>
        /// <remarks>All exceptions thrown while getting types from the assemblies are ignored, and the types that can be retrieved are returned.</remarks>
        public static IEnumerable<Type> FromAssemblies(params Assembly[] assemblies)
        {
            return FromAssemblies(true, assemblies);
        }

        /// <summary>
        /// Returns all visible, non-abstract classes from <paramref name="assemblies" />, and optionally skips errors.
        /// </summary>
        /// <param name="skipOnError"><see langword="true"/> to skip errors; otherwise, <see langword="true"/>.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>
        /// All visible, non-abstract classes.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="assemblies"/> contains <see langword="null"/> elements.</exception>
        /// <remarks>
        /// If <paramref name="skipOnError"/> is <see langword="true"/>, all exceptions thrown while getting types from the assemblies are ignored, and the types 
        /// that can be retrieved are returned; otherwise, the original exception is thrown.
        /// </remarks>
        public static IEnumerable<Type> FromAssemblies(bool skipOnError, params Assembly[] assemblies)
        {
            return FromAssemblies(assemblies, skipOnError);
        }

        /// <summary>
        /// Returns all visible, non-abstract classes from <paramref name="assemblies" />.
        /// </summary>
        /// <param name="skipOnError"><see langword="true"/> to skip errors; otherwise, <see langword="true"/>.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>
        /// All visible, non-abstract classes.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="assemblies"/> contains <see langword="null"/> elements.</exception>
        /// <remarks>
        /// If <paramref name="skipOnError"/> is <see langword="true"/>, all exceptions thrown while getting types from the assemblies are ignored, and the types 
        /// that can be retrieved are returned; otherwise, the original exception is thrown.
        /// </remarks>
        public static IEnumerable<Type> FromAssemblies(IEnumerable<Assembly> assemblies, bool skipOnError = true)
        {
            return FromCheckedAssemblies(CheckAssemblies(assemblies ?? throw new ArgumentNullException(nameof(assemblies))), skipOnError);
        }

        private static IEnumerable<Type> FromCheckedAssemblies(IEnumerable<Assembly> assemblies, bool skipOnError)
        {
            return assemblies
                    .SelectMany(a =>
                    {
                        IEnumerable<TypeInfo> types;

                        try
                        {
                            types = a.DefinedTypes;
                        }
                        catch (ReflectionTypeLoadException e)
                        {
                            if (!skipOnError)
                            {
                                throw;
                            }

                            types = e.Types.TakeWhile(t => t != null).Select(t => t.GetTypeInfo());
                        }

                        return types.Where(ti => ti.IsClass & !ti.IsAbstract && !ti.IsValueType && ti.IsVisible).Select(ti => ti.AsType());
                    });
        }

        private static IEnumerable<Assembly> CheckAssemblies(IEnumerable<Assembly> assemblies)
        {
            var checkAssemblies = assemblies as Assembly[] ?? assemblies.ToArray();
            if (checkAssemblies.Any(assembly => assembly == null))
            {
                throw new ArgumentException(Constants.ExceptionNullAssembly, nameof(assemblies));
            }

            return checkAssemblies;
        }

        private static bool IsSystemAssembly(Assembly a)
        {
            if (NetFrameworkProductName != null)
            {
                var productAttribute = a.GetCustomAttribute<AssemblyProductAttribute>();
                return productAttribute != null && string.Compare(NetFrameworkProductName, productAttribute.Product, StringComparison.Ordinal) == 0;
            }
            return false;
        }

        private static bool IsUnityAssembly(Assembly a)
        {
            if (UnityProductName == null) return false;
            var productAttribute = a.GetCustomAttribute<AssemblyProductAttribute>();
            return productAttribute != null && string.Compare(UnityProductName, productAttribute.Product, StringComparison.Ordinal) == 0;
        }

        private static string GetNetFrameworkProductName()
        {
            var productAttribute = typeof(object).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return productAttribute?.Product;
        }

        private static string GetUnityProductName()
        {
            var productAttribute = typeof(AllClasses).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return productAttribute?.Product;
        }
    }
}
