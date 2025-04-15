// <copyright file="KnownTypeCache.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework
{
    using System.Collections.Concurrent;

    internal static class KnownTypeCache
    {
        private static readonly IDictionary<string, Type> typeCache = new ConcurrentDictionary<string, Type>();

        static KnownTypeCache()
        {
            AddKnownType(typeof(PropertyBag));
            AddKnownType(typeof(SystemObject));
            AddKnownType(typeof(ResourceDescriptor));
            AddKnownType(typeof(SystemObjectMetadata));
        }

        private static void AddKnownType(Type type)
        {
            typeCache.Add(type.Name, type);
        }

        public static Type? ResolveType(string name)
        {
            if (typeCache.TryGetValue(name, out var type))
            {
                // RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                return type;
            }

            return null;
        }
    }
}
