// <copyright file="PlatformRuntimeMiddleware.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.WebHost.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Runtime.WebHost.Command;
    using Kuiper.Platform.Serialization.Serialization;

    using Microsoft.AspNetCore.Http;

    internal class PlatformRuntimeMiddleware : IMiddleware
    {
        private readonly string[] supportedVerbs = new string[] { "GET", "PUT", "POST", "DELETE", "PATCH" };
        private readonly string[] payloadVerbs = new string[] { "PUT", "POST", "PATCH" };

        private readonly PlatformRuntime platformRuntime;

        public PlatformRuntimeMiddleware(PlatformRuntime platformRuntime)
        {
            this.platformRuntime = platformRuntime;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Core Application Logic
                if (context.Request.Method == "POST" &&
                    context.Request.Path == "/api/execute")
                {
                    // Execute the platform runtime
                    await this.ExecuteAsync(context);
                    return;
                }
                else if (this.supportedVerbs.Contains(context.Request.Method) &&
                    context.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCultureIgnoreCase, out PathString remaining))
                {
                    await this.ExecuteVerbAsync(context, remaining);
                    return;
                }
            }
            catch (PlatformRuntimeException ex)
            {
                if (context.Response.HasStarted)
                {
                    return;
                }

                ExecuteResponse response = new ExecuteResponse("Error")
                {
                    ActivityId = context.TraceIdentifier,
                    Status =
                    {
                        Message = ex.Message,
                        Code = ex.HttpResponseCode,
                        Details = [
                            ex.ToString()
                        ],
                    },
                };
                context.Response.StatusCode = ex.HttpResponseCode;
                await response.ObjectToJsonAsync(context.Response.Body, true);

                // DO SOMETHING HERE TO RETURN AN ERROR OBJECT!

                return;
            }

            // Default
            await next(context);
        }

        private async Task ExecuteVerbAsync(HttpContext context, PathString subPath)
        {
            // Convert Context into platform request
            PlatformRequest? platformRequest = null;
            _ = ResourcePathParser.TryParse(subPath, out ResourceDescriptor? resourceDescriptor);
            bool isPayloadVerb = this.payloadVerbs.Contains(context.Request.Method);
            bool allowSystemModification = context.Request.Headers.TryGetValue("x-allow-system-modification", out var value) &&
                bool.TryParse(value, out var boolValue) &&
                boolValue;

            if (isPayloadVerb && context.Request.ContentType != "application/json")
            {
                context.Response.StatusCode = 415;
                return;
            }

            switch (context.Request.Method)
            {
                case "GET":
                    platformRequest = CreateGetRequest(resourceDescriptor);
                    break;
                case "PUT":
                    platformRequest = await CreatePutRequestAsync(context, resourceDescriptor, allowSystemModification);
                    break;
                case "DELETE":
                    platformRequest = CreateDeleteRequest(resourceDescriptor, allowSystemModification);
                    break;
                default:
                    context.Response.StatusCode = 405;
                    return;
            }

            if (platformRequest == null)
            {
                context.Response.StatusCode = 400;
                return;
            }

            // No, trace-id is not the same as activity-id or the internal 'TraceIdentifier' on http-context
            context.Request.Headers.TryGetValue("x-trace-id", out var traceId);
            platformRequest.PrepareRequest(context.TraceIdentifier, traceId);

            var result = await this.platformRuntime.ExecuteAsync(platformRequest, context.RequestAborted);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Status.Code;

            await result.ObjectToJsonAsync(context.Response.Body, true);
        }

        private async Task ExecuteAsync(HttpContext context)
        {
            if (context.Request.ContentType != "application/json")
            {
                context.Response.StatusCode = 415;
                return;
            }

            // Convert Context into platform request
            var platformRequest = await context.Request.Body.ObjectFromJsonAsync<PlatformRequest>();

            if (string.IsNullOrWhiteSpace(platformRequest.ActivityId) &&
                context.Request.Headers.TryGetValue("x-request-id", out var value))
            {
                platformRequest.ActivityId = value!;
            }

            if (string.IsNullOrWhiteSpace(platformRequest.ActivityId))
            {
                platformRequest.ActivityId = Guid.NewGuid().ToString();
            }

            context.TraceIdentifier = platformRequest.ActivityId;

            var result = await this.platformRuntime.ExecuteAsync(platformRequest, context.RequestAborted);

            context.Response.Headers.Append("x-request-id", platformRequest.ActivityId);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Status.Code;

            await result.ObjectToJsonAsync(context.Response.Body, true);
        }

        private static PlatformRequest CreateGetRequest(ResourceDescriptor? resourceDescriptor)
        {
            string message = string.IsNullOrWhiteSpace(resourceDescriptor!.Name) ? "List" : "Get";
            return new PlatformRequest(message, resourceDescriptor);
        }

        private static PlatformRequest CreateDeleteRequest(ResourceDescriptor? resourceDescriptor, bool allowSystemModification)
        {
            return new PlatformRequest("Delete", resourceDescriptor)
            {
                InputParameters =
                {
                    ["allowSystemModification"] = allowSystemModification,
                },
            };
        }

        private static async Task<PlatformRequest?> CreatePutRequestAsync(HttpContext context, ResourceDescriptor? resourceDescriptor, bool allowSystemModification)
        {
            var target = await context.Request.Body.ObjectFromJsonAsync<SystemObject>();

            if (resourceDescriptor != null)
            {
                target.Metadata = target.Metadata ?? new SystemObjectMetadata();
                target.Kind = resourceDescriptor.Kind;
                target.Metadata.Namespace = resourceDescriptor.Namespace;
                target.Metadata.Name = resourceDescriptor.Name ?? target.Metadata.Name;
            }

            var request = new PlatformRequest("Put", target)
            {
                InputParameters =
                {
                    ["allowSystemModification"] = allowSystemModification,
                },
            };

            return request;
        }
    }
}
