// <copyright file="PlatformRequestMiddleware.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Middleware
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Runtime.WebHost.Command;
    using Kuiper.Platform.Serialization.Serialization;

    public class PlatformRequestMiddleware : IMiddleware
    {
        private readonly string[] supportedVerbs = new string[] { "GET", "PUT", "POST", "DELETE", "PATCH" };
        private readonly string[] payloadVerbs = new string[] { "PUT", "POST", "PATCH" };

        private readonly PlatformRuntime platformRuntime;

        public PlatformRequestMiddleware(PlatformRuntime platformRuntime)
        {
            this.platformRuntime = platformRuntime;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Core Application Logic
                if (context.Request.Method == "POST" &&
                    context.Request.Path == "/api/v1/kuiper")
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
                    RequestId = context.TraceIdentifier,
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
            ResourcePathParser.TryParse(subPath, out ResourceDescriptor? resourceDescriptor);
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
                    platformRequest = this.CreateGetRequest(resourceDescriptor);
                    break;
                case "PUT":
                    platformRequest = await this.CreatePutRequestAsync(context, resourceDescriptor, allowSystemModification);
                    break;
                //case "POST":
                //    platformRequest = await this.CreatePostRequestAsync(context, resourceDescriptor);
                //    break;
                case "DELETE":
                    platformRequest = this.CreateDeleteRequest(resourceDescriptor, allowSystemModification);
                    break;
                //case "PATCH":
                //    platformRequest = await this.CreatePatchRequestAsync(context, resourceDescriptor);
                //    break;
                default:
                    context.Response.StatusCode = 405;
                    return;
            }

            if (platformRequest == null)
            {
                context.Response.StatusCode = 400;
                return;
            }

            // REQUEST
            context.Request.Headers.TryGetValue("x-trace-id", out var traceId);
            platformRequest.PrepareRequest(traceId);
            context.TraceIdentifier = platformRequest.RequestId ?? context.TraceIdentifier;

            var result = await this.platformRuntime.ExecuteAsync(platformRequest, context.RequestAborted);

            // RESPONSE
            if (!string.IsNullOrWhiteSpace(platformRequest.RequestId))
            {
                context.Response.Headers.Append("x-request-id", platformRequest.RequestId);
            }

            if (!string.IsNullOrWhiteSpace(platformRequest.TraceId))
            {
                context.Response.Headers.Append("x-trace-id", platformRequest.TraceId);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Status.Code;

            await result.ObjectToJsonAsync(context.Response.Body, true);
        }

        private PlatformRequest CreateGetRequest(ResourceDescriptor? resourceDescriptor)
        {
            string message = string.IsNullOrWhiteSpace(resourceDescriptor!.Name) ? "List" : "Get";
            return new PlatformRequest(message, resourceDescriptor);
        }

        private PlatformRequest CreateDeleteRequest(ResourceDescriptor? resourceDescriptor, bool allowSystemModification)
        {
            return new PlatformRequest("Delete", resourceDescriptor)
            {
                InputParameters =
                {
                    ["allowSystemModification"] = allowSystemModification,
                },
            };
        }

        private async Task<PlatformRequest?> CreatePutRequestAsync(HttpContext context, ResourceDescriptor? resourceDescriptor, bool allowSystemModification)
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

        private async Task ExecuteAsync(HttpContext context)
        {
            if (context.Request.ContentType != "application/json")
            {
                context.Response.StatusCode = 415;
                return;
            }

            // Convert Context into platform request
            var platformRequest = await context.Request.Body.ObjectFromJsonAsync<PlatformRequest>();

            if (string.IsNullOrWhiteSpace(platformRequest.RequestId) &&
                context.Request.Headers.TryGetValue("x-request-id", out var value))
            {
                platformRequest.RequestId = value!;
            }

            if (string.IsNullOrWhiteSpace(platformRequest.RequestId))
            {
                platformRequest.RequestId = Guid.NewGuid().ToString();
            }

            context.TraceIdentifier = platformRequest.RequestId;

            var result = await this.platformRuntime.ExecuteAsync(platformRequest, context.RequestAborted);

            context.Response.Headers.Append("x-request-id", platformRequest.RequestId);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Status.Code;

            await result.ObjectToJsonAsync(context.Response.Body, true);
        }
    }
}
