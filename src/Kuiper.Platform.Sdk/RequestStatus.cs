// <copyright file="RequestStatus.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk
{
    using System.Net;

    public sealed class RequestStatus
    {
        // Numerical status code, like an HTTP status code
        public int Code { get; set; } = (int)HttpStatusCode.OK;

        public bool IsSuccess()
        {
            return this.Code == (int)HttpStatusCode.OK;
        }

        // A human-readable message that provides more context about the status
        public string? Message { get; set; }

        // A list of details, where each detail can be a dynamic object (similar to Protobuf's Any type)
        public IList<object> Details { get; set; } = new List<object>();

        public RequestStatus() { }

        public RequestStatus(int code = (int)HttpStatusCode.OK, string? message = null, IList<object>? details = null)
        {
            this.Code = code;
            this.Message = message;

            if (details != null)
            {
                this.Details = details;
            }
        }

        // ToString method to format the status for easy printing/logging
        public override string ToString()
        {
            return $"Code: {this.Code}, Message: {this.Message ?? "<none>"}, Details Count: {this.Details.Count}";
        }
    }
}
