using Kuiper.Platform.Runtime.WebHost.Middleware;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Kuiper.Platform.Runtime.WebHost
{
    internal static class WebHostEndpointBuilder
    {
        public static IEndpointRouteBuilder MapKuiperWebHostRuntimeEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var pipeline = endpoints
                .CreateApplicationBuilder()
                .UseMiddleware<KuiperWebHostMiddleware>()
                .Use(async (HttpContext context, RequestDelegate next) =>
                {
                    context.Response.StatusCode = 404; // Set the response status code to 404
                    await context.Response.CompleteAsync(); // Ensure the response is completed and no further middleware is executed
                })
                .Build();

            endpoints.Map($"/api/{{*pathInfo}}", pipeline)
                .WithDescription("Kuiper WebHost Runtime Request Handler");

            // Well known endpoint discovery
            // TODO

            return endpoints;
        }
    }
}
