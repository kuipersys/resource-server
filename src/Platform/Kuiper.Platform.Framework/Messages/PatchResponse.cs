// <copyright file="PatchResponse.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Messages
{
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework;
    using Kuiper.Platform.ManagementObjects;

    public class PatchResponse : PlatformResponse
    {
        public PatchResponse()
            : base(Constants.Patch)
        {
        }

        [JsonPropertyOrder(10)]
        public ResourceDescriptor? Reference { get; set; }

        [JsonPropertyOrder(10)]
        public SystemObject? Result { get; set; }
    }
}
