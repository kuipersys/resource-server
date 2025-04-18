// <copyright file="HookData.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    public sealed class ResourceOperationHookSpec
    {
        [Required]
        [JsonPropertyName("target")]
        public HookTargetSelector Target { get; set; } = new HookTargetSelector();

        public HookExecutionMode Mode { get; init; }
        [Required]
        [JsonPropertyName("operation")]
        public ResourceOperation Operation { get; set; } = default!;

        [Required]
        [JsonPropertyName("phase")]
        public HookPhase Phase { get; set; } = default!;

        [Required]
        [JsonPropertyName("blocking")]
        public bool Blocking { get; set; } = false;

        [Required]
        [JsonPropertyName("priority")]
        public int Priority { get; set; } = 100_000_000;

        [JsonPropertyName("endpoint")]
        public HookEndpoint? Endpoint { get; init; }

        [JsonPropertyName("plugin")]
        public HookPlugin? Plugin { get; init; }
    }
}
