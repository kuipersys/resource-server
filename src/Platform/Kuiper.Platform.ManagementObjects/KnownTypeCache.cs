// <copyright file="KnownTypeCache.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects
{
    using System;
    using System.Collections.Generic;

    using Kuiper.Platform.Serialization;
    using Kuiper.Platform.Serialization.Serialization;

    internal static class KnownTypeCache
    {
        private static readonly IDictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        static KnownTypeCache()
        {
            AddKnownType(typeof(PropertyBag));
            AddKnownType(typeof(SystemObject));
            AddKnownType(typeof(ResourceDescriptor));
            AddKnownType(typeof(SystemObjectMetadata));
            SerializationSettings.AddConverter(new PropertyBagConverter());
        }

        public static Type? ResolveType(string name)
        {
            if (TypeCache.TryGetValue(name, out var type))
            {
                return type;
            }

            return null;
        }

        private static void AddKnownType(Type type)
        {
            TypeCache.Add(type.Name, type);
        }
    }
}
