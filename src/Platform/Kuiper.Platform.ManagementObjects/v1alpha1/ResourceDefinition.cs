// <copyright file="ResourceDefinition.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Abstractions.Objects;

    public sealed class ResourceDefinition : SystemObject, IResourceDefinition
    {
        [Required]
        [JsonPropertyOrder(100)]
        [JsonPropertyName("spec")]
        public ResourceDefinitionSpec Spec { get; set; } = new ResourceDefinitionSpec();

        public string GetKey()
        {
            return $"{this.Spec.Group}/{this.Spec.Names.Kind}";
        }

        public IDictionary<string, TVersionObject> GetVersions<TVersionObject>()
            where TVersionObject : class, IResourceDefinitionVersion
        {
            IDictionary<string, TVersionObject> versions = new Dictionary<string, TVersionObject>();

            foreach (var version in this.Spec.Versions)
            {
                var versionObject = version as TVersionObject;

                if (versionObject == null)
                {
                    throw new InvalidCastException($"Cannot cast {version.GetType()} to {typeof(TVersionObject)}");
                }

                if (version.Enabled)
                {
                    versions.Add($"{this.GetKey()}/{version.Name}", versionObject);
                }
            }

            return new ReadOnlyDictionary<string, TVersionObject>(versions);
        }
    }
}
