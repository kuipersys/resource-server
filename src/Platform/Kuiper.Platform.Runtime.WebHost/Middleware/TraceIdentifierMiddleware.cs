// <copyright file="TraceIdentifierMiddleware.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.WebHost.Middleware
{
    using System;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Errors;

    using Microsoft.AspNetCore.Http;

    internal class TraceIdentifierMiddleware : IMiddleware
    {
        private const int TraceIdMaxLength = 128;

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Override the default TraceIdentifier with a GUID-based value
            context.TraceIdentifier = Guid.NewGuid().ToString("D"); // no dashes

            // Remove the activity id header if it exists
            if (context.Request.Headers.ContainsKey("X-Activity-Id"))
            {
                // Remove the header to ensure that this value is sytem generated
                context.Request.Headers.Remove("X-Activity-Id");
            }

            // Set the activity id header to the system trace identifier we just created
            context.Request.Headers["X-Activity-Id"] = context.TraceIdentifier;

            // If a client provided trace-id, store it for later use
            string? traceId = null;
            if (context.Request.Headers.TryGetValue("X-Trace-Id", out var traceIdHeader))
            {
                traceId = traceIdHeader.ToString();

                if (traceId.Length > TraceIdMaxLength)
                {
                    throw new PlatformRuntimeException($"Trace ID is too long. Maximum length is {TraceIdMaxLength} characters.", errorCode: PlatformRuntimeErrorCodes.BadRequest);
                }

                // ensure that the trace-id is alphanumeric, dashes, and underscores and dots
                if (!System.Text.RegularExpressions.Regex.IsMatch(traceId, @"^[a-zA-Z0-9_.-]+$"))
                {
                    throw new PlatformRuntimeException($"Trace ID contains invalid characters. Only alphanumeric characters, dashes, underscores, and periods are allowed.", errorCode: PlatformRuntimeErrorCodes.BadRequest);
                }
            }

            // Optionally add it to response headers for correlation
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Activity-Id"] = context.TraceIdentifier;

                // If a client provided trace-id, add it to the response headers
                if (!string.IsNullOrEmpty(traceId))
                {
                    context.Response.Headers["X-Trace-Id"] = traceId;
                }

                return Task.CompletedTask;
            });

            return next(context);
        }
    }
}
