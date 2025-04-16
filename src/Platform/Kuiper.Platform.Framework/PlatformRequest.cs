// <copyright file="PlatformRequest.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.Serialization;

    [JsonDerivedType(typeof(PutRequest), nameof(PutRequest))]
    [JsonDerivedType(typeof(GetRequest), nameof(GetRequest))]
    [JsonDerivedType(typeof(PatchRequest), nameof(PatchRequest))]
    [JsonDerivedType(typeof(DeleteRequest), nameof(DeleteRequest))]
    [JsonDerivedType(typeof(ExecuteRequest), nameof(ExecuteRequest))]
    public class PlatformRequest
    {
        public const int MAX_TRACE_ID_LENGTH = 128;

        public PlatformRequest()
        {
        }

        public PlatformRequest(string message)
        {
            this.Message = message;
        }

        public PlatformRequest(string message, object target)
        {
            this.Message = message;
            this.InputParameters["target"] = target;
        }

        [JsonPropertyOrder(1)]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyOrder(2)]
        public string? RequestId { get; set; }

        [JsonPropertyOrder(3)]
        public string? TraceId { get; set; }

        [JsonPropertyOrder(4)]
        public PropertyBag InputParameters { get; set; } = new PropertyBag();

        public T GetTarget<T>()
        {
            if (this.InputParameters.TryGetValue("target", out object target))
            {
                return (T)target;
            }

            throw new InvalidOperationException($"Target not found in {this.Message} request.");
        }

        protected static string ConvertName(string name)
            => JsonNamingPolicy.CamelCase.ConvertName(name);

        public void PrepareRequest(string? traceId = null)
        {
            this.InputParameters["$type"] = $"{this.Message}Request";

            // Set Request Id
            this.RequestId = this.RequestId ?? Guid.NewGuid().ToString();

            // Initialize Trace Id and Trim
            this.TraceId = traceId ?? this.TraceId;
            this.NormalizeTraceId();
        }

        private void NormalizeTraceId()
        {
            if (this.TraceId == null || this.TraceId.Length <= MAX_TRACE_ID_LENGTH)
            {
                return;
            }

            this.TraceId = this.TraceId.Substring(0, MAX_TRACE_ID_LENGTH);
        }
    }
}
