// <copyright file="PlatformResponse.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework
{
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.ManagementObjects;

    [JsonDerivedType(typeof(PutResponse), nameof(PutResponse))]
    [JsonDerivedType(typeof(GetResponse), nameof(GetResponse))]
    [JsonDerivedType(typeof(PatchResponse), nameof(PatchResponse))]
    [JsonDerivedType(typeof(DeleteResponse), nameof(DeleteResponse))]
    [JsonDerivedType(typeof(ExecuteResponse), nameof(ExecuteResponse))]
    public class PlatformResponse
    {
        public PlatformResponse()
        {
        }

        public PlatformResponse(string message)
        {
            this.Message = message;
        }

        [JsonPropertyOrder(1)]
        public string Message { get; set; }

        [JsonPropertyOrder(2)]
        public string RequestId { get; set; }

        [JsonPropertyOrder(3)]
        public RequestStatus Status { get; set; } = new RequestStatus();

        [JsonExtensionData]
        public PropertyBag ExtensionData { get; set; } = new PropertyBag();
    }
}
