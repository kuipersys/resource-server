// <copyright file="PlatformException.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Errors
{
    public class PlatformRuntimeException : Exception
    {
        public string? ErrorCode { get; }

        public int HttpResponseCode => PlatformRuntimeErrorCodes.HttpResponseCodes.GetValueOrDefault(this.ErrorCode ?? string.Empty, 500);

        public PlatformRuntimeException(string errorCode, Exception? innerException = null)
            : base(PlatformRuntimeErrorCodes.ErrorMessages.GetValueOrDefault(errorCode) ?? nameof(PlatformRuntimeException), innerException)
        {
            this.ErrorCode = errorCode;
        }

        public PlatformRuntimeException(string message, string? errorCode, Exception? innerException = null)
            : base(message, innerException)
        {
            this.ErrorCode = string.IsNullOrWhiteSpace(errorCode) ? PlatformRuntimeErrorCodes.BadRequest : errorCode;
        }
    }
}
