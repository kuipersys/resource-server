// <copyright file="DependencyInjection.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    using Kuiper.Platform.Runtime.Abstractions.Command;
    using Kuiper.Platform.Runtime.Command;

    public static class KuiperRuntimeDependencyInjection
    {
        public static IServiceCollection AddKuiperRuntime(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

            return services;
        }
    }
}
