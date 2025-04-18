// <copyright file="KuiperServicesExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using Kuiper.ResourceServer.Service.Core;
using Kuiper.ServiceInfra.Abstractions.Persistence;
using Kuiper.ServiceInfra.Persistence;

namespace Kuiper.ResourceServer.Service.Middleware;

public static class KuiperServicesExtensions
{
    public static async Task<IServiceCollection> AddKuiperServicesAsync(this IServiceCollection services, IConfiguration configuration, KuiperEndpointConfiguration? endpointConfiguration = null)
    {
        var store = CreateKeyValueStore(configuration);
        var resourceManager = new ResourceManager(store);
        await resourceManager.InitializeAsync();

        services
            .AddSingleton(store)
            .AddSingleton<IResourceManager>(resourceManager)
            .AddScoped<PlatformRequestMiddleware>()
            .AddPluginRuntime()
            .AddSingleton(endpointConfiguration ?? new KuiperEndpointConfiguration());

        return services;
    }

    private static IKeyValueStore CreateKeyValueStore(IConfiguration configuration)
    {
        string path = configuration["Kuiper:KeyValueStore:Path"] ?? "./data";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return FileSystemKeyValueStore.Create(path);
    }

    /// <summary>
    /// This is the default handler for all un-mapped requests
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="endpointConfiguration"></param>
    /// <returns></returns>
    private static IEndpointConventionBuilder MapPlatformApiEndpoints(
        this IEndpointRouteBuilder endpoints,
        KuiperEndpointConfiguration endpointConfiguration)
    {
        var pipeline = endpoints
            .CreateApplicationBuilder()
            .UseMiddleware<PlatformRequestMiddleware>()
            .Use(async (HttpContext context, RequestDelegate next) =>
            {
                context.Response.StatusCode = 404; // Set the response status code to 404
                await context.Response.CompleteAsync(); // Ensure the response is completed and no further middleware is executed
            })
            .Build();

        return endpoints.Map($"/{{*pathInfo}}", pipeline)
            .WithDisplayName("Kuiper Platform: Platform Request Handler");
    }

    public static IEndpointRouteBuilder MapKuiperServicesEndpoints(this WebApplication app)
    {
        KuiperEndpointConfiguration endpointConfiguration =
            app.Services.GetRequiredService<KuiperEndpointConfiguration>();

        return app.MapKuiperServicesEndpoints(endpointConfiguration);
    }

    private static IEndpointRouteBuilder MapKuiperServicesEndpoints(this IEndpointRouteBuilder endpoints, KuiperEndpointConfiguration endpointConfiguration)
    {
        // Always last
        endpoints.MapPlatformApiEndpoints(endpointConfiguration);

        return endpoints;
    }
}
