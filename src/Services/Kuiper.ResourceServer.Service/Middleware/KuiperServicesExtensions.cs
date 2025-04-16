// <copyright file="KuiperServicesExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using Kuiper.Platform.Framework.Abstractions;
using Kuiper.Platform.ManagementObjects.v1alpha1;
using Kuiper.ResourceServer.Service;
using Kuiper.ResourceServer.Service.Core;
using Kuiper.ServiceInfra.Abstractions.Persistence;
using Kuiper.ServiceInfra.Persistence;

namespace Kuiper.ResourceServer.Service.Middleware;

public static class KuiperServicesExtensions
{
    public static IServiceCollection AddKuiperServices(this IServiceCollection services, KuiperEndpointConfiguration? endpointConfiguration = null)
    {
        services
            .AddSingleton(services =>
            {
                var store = services.GetRequiredService<IKeyValueStore>();
                var builder = new ResourceDefinitionManagerBuilder(store, new CoreResourceModule());

                var modules = services.GetServices<IPlatformModule>();
                if (modules != null && modules.Count() > 0)
                {
                    foreach (var module in modules)
                    {
                        builder.RegisterModule(module);
                    }
                }

                return builder;
            })
            .AddSingleton<IResourceManager>(services =>
            {
                var builder = services.GetRequiredService<ResourceDefinitionManagerBuilder>();
                ResourceManager resourceManager = new ResourceManager(services.GetRequiredService<IKeyValueStore>());
                Task initTask = resourceManager.InitializeAsync(builder);
                initTask.Wait(CancellationToken.None);

                if (initTask.IsFaulted)
                {
                    throw initTask.Exception ?? new Exception("Failed to initialize resource manager");
                }

                if (initTask.IsCanceled)
                {
                    throw new Exception("Failed to initialize resource manager");
                }

                return resourceManager;
            })
            .AddSingleton<IResourceRequestValidator, ResourceRequestValidator>()
            .AddScoped<PlatformRequestMiddleware>()
            .AddPluginRuntime()
            .AddSingleton(services => FileSystemKeyValueStore.Create("C:\\CloudApi\\ResourceProvider\\Data"))
            .AddSingleton(endpointConfiguration ?? new KuiperEndpointConfiguration());

        return services;
    }

    /// <summary>
    /// This is the default handler for all un-mapped requests
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="endpointConfiguration"></param>
    /// <returns></returns>
    private static IEndpointConventionBuilder MapPlatformApiEndpoints(this IEndpointRouteBuilder endpoints,
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
