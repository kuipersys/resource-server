// <copyright file="ResourceDefinition.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.ManagementObjects;

    public sealed class ResourceDefinition : SystemObject
    {
        [Required]
        [JsonPropertyOrder(100)]
        [JsonPropertyName("spec")]
        public ResourceDefinitionSpec Spec { get; set; } = new ResourceDefinitionSpec();

        public string GetKey()
        {
            return $"{this.Spec.Group}/{this.Spec.Names.Kind}";
        }

        public IDictionary<string, ResourceDefinitionVersion> GetVersions()
        {
            IDictionary<string, ResourceDefinitionVersion> versions = new Dictionary<string, ResourceDefinitionVersion>();

            foreach (var version in this.Spec.Versions)
            {

                if (version.Enabled)
                {
                    versions.Add($"{this.GetKey()}/{version.Name}", version);
                }
            }

            return new ReadOnlyDictionary<string, ResourceDefinitionVersion>(versions);
        }
    }
}
