// <copyright file="PlatformException.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Errors
{
    public class PlatformException : Exception
    {
        public string? ErrorCode { get; }

        public int HttpResponseCode => PlatformErrorCodes.HttpResponseCodes.GetValueOrDefault(this.ErrorCode ?? string.Empty, 500);

        public PlatformException(string errorCode, Exception? innerException = null)
            : base(PlatformErrorCodes.ErrorMessages.GetValueOrDefault(errorCode) ?? nameof(PlatformException), innerException)
        {
            this.ErrorCode = errorCode;
        }

        public PlatformException(string message, string? errorCode, Exception? innerException = null)
            : base(message, innerException)
        {
            this.ErrorCode = string.IsNullOrWhiteSpace(errorCode) ? PlatformErrorCodes.BadRequest : errorCode;
        }
    }
}
