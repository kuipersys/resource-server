namespace Kuiper.Platform.Runtime.WebHost.Middleware
{
    using System.Text;
    using System.Threading.Tasks;

    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.Runtime.Abstractions.Command;
    using Kuiper.Platform.Runtime.WebHost.Command;
    using Kuiper.Platform.Serialization.Serialization;

    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    internal class KuiperWebHostMiddleware : IMiddleware
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IHttpContextCommandFactory httpContextCommandFactory;

        public KuiperWebHostMiddleware(ICommandDispatcher commandDispatcher, IHttpContextCommandFactory httpContextCommandFactory)
        {
            this.commandDispatcher = commandDispatcher;
            this.httpContextCommandFactory = httpContextCommandFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            ICommandContext? commandContext = await httpContextCommandFactory.CreateAsync(context);

            if (commandContext == null)
            {
                return;
            }

            ICommandResult result = await commandDispatcher.DispatchAsync(commandContext);
            await WriteAsync(context, result);
        }

        private static async Task WriteAsync(HttpContext httpContext, ICommandResult result)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = result.StatusCode ?? (result.IsSuccess ? 200 : 400);

            PlatformResponse platformResponse = new PlatformResponse(httpContext.Request.Method);

            if (result.Data != null)
            {
                platformResponse.ExtensionData["data"] = result.Data;
            }

            await platformResponse.ObjectToJsonAsync(httpContext.Response.Body, true);
        }
    }
}