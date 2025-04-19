// <copyright file="KuiperWebHostRuntimeDependencyInjection.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    using System;

    using Kuiper.Platform.Runtime.WebHost;
    using Kuiper.Platform.Runtime.WebHost.BackgroundJobs;
    using Kuiper.Platform.Runtime.WebHost.Middleware;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KuiperWebHostRuntimeDependencyInjection
    {
        public static IServiceCollection AddKuiperWebHostRuntime(this IServiceCollection services)
        {
            services.RemoveAll<AspNetCore.Mvc.Infrastructure.IActionContextAccessor>();
            services.RemoveAll<AspNetCore.Mvc.Infrastructure.IActionInvokerFactory>();

            ArgumentNullException.ThrowIfNull(services);

            services.AddAuthorization();
            services.AddHealthChecks();
            services.AddKuiperRuntime();

            services.AddHostedService<RuntimeInitializationService>();

            // Internal Services
            services.AddSingleton<PlatformRuntimeMiddleware>();
            services.AddSingleton<TraceIdentifierMiddleware>();

            // Add any additional services specific to the web host runtime here.
            return services;
        }

        public static WebApplication UseKuiperWebHostRuntime(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            // Configure the middleware pipeline for the web host runtime.
            app.UseAuthorization();
            app.MapKuiperWebHostRuntimeEndpoints();
            app.MapHealthChecks("/healthz");

            // Add any additional middleware specific to the web host runtime here.
            return app;
        }
    }
}
