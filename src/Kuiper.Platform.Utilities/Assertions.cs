// <copyright file="Assertions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Utilities
{
    using System;
    using System.Collections.Generic;

    public static class Assertions
    {
        public static void ThrowIfKeyNotFound<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Exception exception = null)
        {
            if (!dictionary.ContainsKey(key))
            {
                throw exception ?? new KeyNotFoundException($"Key '{key}' not found in dictionary.");
            }
        }

        public static void ThrowIfNull<TObject>(this TObject @object, string message = null)
            => @object.ThrowIfNull(message == null ? null : new NullReferenceException(message));

        public static void ThrowIfNull<TObject>(this TObject @object, Exception exception = null)
        {
            if (@object == null)
            {
                throw exception ?? new NullReferenceException("Object is null.");
            }
        }

        public static void ThrowIfNotAssignableTo<TObject, TType>(this TObject @object, Exception exception = null)
            => ThrowIfNotAssignableTo(@object, typeof(TType), exception);

        public static void ThrowIfNotAssignableTo(this object @object, Type targetType, Exception exception = null)
        {
            if (!targetType.IsAssignableFrom(@object?.GetType()))
            {
                throw exception ?? new InvalidCastException($"{@object?.GetType().FullName ?? "null"} is not assignable to {targetType.FullName}");
            }
        }
    }
}
