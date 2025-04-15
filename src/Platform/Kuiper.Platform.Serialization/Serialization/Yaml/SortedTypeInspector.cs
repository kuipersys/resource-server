// <copyright file="SortedTypeInspector.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Serialization.Serialization.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.TypeInspectors;

    public class SortedTypeInspector : TypeInspectorSkeleton
    {
        private readonly ITypeInspector innerTypeInspector;

        public SortedTypeInspector(ITypeInspector innerTypeInspector)
        {
            this.innerTypeInspector = innerTypeInspector;
        }

        public override string GetEnumName(Type enumType, string name)
        {
            return name;
        }

        public override string GetEnumValue(object enumValue)
        {
            return enumValue.ToString();
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            var orderedProperties = this.innerTypeInspector
                .GetProperties(type, container)
                .Where(x => x.GetCustomAttribute<JsonPropertyNameAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<JsonPropertyOrderAttribute>().Order)
                .ToArray();

            var unOrderedProperties = this.innerTypeInspector
                .GetProperties(type, container)
                .Where(x => x.GetCustomAttribute<JsonPropertyNameAttribute>() == null)
                .OrderBy(x => x.Name)
                .ToArray();

            return orderedProperties.Concat(unOrderedProperties);
        }
    }
}
