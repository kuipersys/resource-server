// <copyright file="PlatformErrorCodes.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Errors
{
    using System.Net;

    public static class PlatformRuntimeErrorCodes
    {
        public const string BadRequest = "BAD_REQUEST";

        public const string InternalServerError = "INTERNAL_SERVER_ERROR";

        public const string InvalidResourceKind = "INVALID_RESOURCE_KIND";

        public const string Forbidden = "FORBIDDEN";

        public const string Unauthorized = "UNAUTHORIZED";

        public const string ResourceNotFound = "RESOURCE_NOT_FOUND";

        public const string ValidationError = "VALIDATION_ERROR";

        public static IReadOnlyDictionary<string, int> HttpResponseCodes { get; } = new Dictionary<string, int>
        {
            { InvalidResourceKind, (int)HttpStatusCode.BadRequest },
            { ResourceNotFound, (int)HttpStatusCode.NotFound },
            { ValidationError, (int)HttpStatusCode.BadRequest },
            { BadRequest, (int)HttpStatusCode.BadRequest },
            { Unauthorized, (int)HttpStatusCode.Unauthorized },
            { Forbidden, (int)HttpStatusCode.Forbidden },
            { InternalServerError, (int)HttpStatusCode.InternalServerError },
        };

        public static IReadOnlyDictionary<string, string> ErrorMessages { get; } = new Dictionary<string, string>
        {
            { InvalidResourceKind, "Invalid resource kind." },
            { ResourceNotFound, "Resource not found." },
            { ValidationError, "Validation error." },
            { BadRequest, "Bad request." },
            { InternalServerError, "Internal server error." },
            { Unauthorized, "Unauthorized." },
            { Forbidden, "Forbidden." },
        };
    }
}
