// <copyright file="HttpContextCommandFactory.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.WebHost.Command
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Abstractions.Command;
    using Kuiper.Platform.Runtime.Command;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ResourceServer.Service.Core;

    using Microsoft.AspNetCore.Http;

    internal sealed class HttpContextCommandFactory : IHttpContextCommandFactory
    {
        private static IDictionary<string, string> MethodMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "GET", "get" },
            { "POST", "execute" },
            { "PUT", "create" },
            { "DELETE", "delete" },
            { "PATCH", "patch" },
        };

        public async Task<ICommandContext?> CreateAsync(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCultureIgnoreCase, out PathString remaining))
            {
                httpContext.Response.StatusCode = 404; // Not Found
                return null;
            }

            var commandName = GetHttpCommandName(httpContext.Request.Method, remaining);
            (bool @continue, JsonElement? payload) = await TryGetPayloadAsync(httpContext);

            if (!@continue)
            {
                return null;
            }

            var metadata = new Dictionary<string, object>
            {
                ["HttpMethod"] = httpContext.Request.Method,
                ["Path"] = remaining.ToString().TrimStart('/'),
                ["UserAgent"] = httpContext.Request.Headers["User-Agent"].ToString(),
                ["RemoteIp"] = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            };

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                metadata["UserId"] = httpContext.User.Identity.Name ?? "unknown";
            }

            var arguments = new Dictionary<string, object>();
            var resourcePathParser = ResourcePathParser.Parse(remaining);

            if (resourcePathParser == null)
            {
                httpContext.Response.StatusCode = 400; // Bad Request
                return null;
            }

            // Special case for GET requests without a name of the resource
            if (commandName == "get" && string.IsNullOrWhiteSpace(resourcePathParser.Name))
            {
                commandName = "list";
            }

            var parsedCommand = new ParsedCommand()
            {
                CommandName = commandName,
                NamedArguments =
                {
                    ["kind"] = resourcePathParser.Kind,
                    ["namespace"] = resourcePathParser.Namespace,
                    ["group"] = resourcePathParser.Group,
                }
            };

            return new CommandContext(
                httpContext.RequestServices,
                command: parsedCommand,
                payload: payload,
                cancellationToken: httpContext.RequestAborted,
                correlationId: httpContext.TraceIdentifier,
                source: "HTTP",
                metadata: metadata
            );
        }

        private static string GetHttpCommandName(string method, PathString path)
        {
            if (!MethodMap.TryGetValue(method, out string? commandName))
            {
                commandName = "unknown";
            }

            return $"{commandName}";
        }

        private static async Task<(bool, JsonElement?)> TryGetPayloadAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            bool isJsonContentType = context.Request.ContentType == "application/json";

            if (context.Request.Body.Length > 0 && !isJsonContentType)
            {
                context.Response.StatusCode = 415; // Unsupported Media Type
                await context.Response.WriteAsync("Unsupported content type. Expected application/json.");
                return (false, null);
            }

            if (!isJsonContentType)
            {
                return (true, null);
            }

            var payload = await context.Request.Body.StreamToJsonElementAsync();
            context.Request.Body.Position = 0;

            return (true, payload);
        }
    }
}
