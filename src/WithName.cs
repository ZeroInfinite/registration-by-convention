﻿

using System;

namespace Unity.RegistrationByConvention
{
    /// <summary>
    /// Provides helper methods to get type names.
    /// </summary>
    public static class WithName
    {
        /// <summary>
        /// Returns the type name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type name.</returns>
        public static string TypeName(Type type)
        {
            return (type ?? throw new ArgumentNullException(nameof(type))).Name;
        }

        /// <summary>
        /// Returns null for the registration name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><see langword="null"/></returns>
        public static string Default(Type type)
        {
            return null;
        }
    }
}
