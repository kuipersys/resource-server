// <copyright file="ResourceOperationHook.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    public sealed class ResourceOperationHook : SystemObject
    {
        [Required]
        [JsonPropertyOrder(100)]
        [JsonPropertyName("spec")]
        public ResourceOperationHookSpec Spec { get; set; } = new ResourceOperationHookSpec();
    }
}
