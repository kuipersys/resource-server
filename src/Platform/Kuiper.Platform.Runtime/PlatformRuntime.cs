// <copyright file="PlatformRuntime.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Runtime.Abstractions.Extensibility;
    using Kuiper.Platform.Runtime.Errors;
    using Kuiper.Platform.Runtime.Execution;
    using Kuiper.Platform.Runtime.Extensibility;

    using Microsoft.Extensions.DependencyInjection;

    public class PlatformRuntime
    {
        private readonly IPluginRuntime pluginRuntime;
        private readonly Func<IServiceCollection, Task> configureRuntimeServices;

        public PlatformRuntime(IPluginRuntime pluginRuntime, Func<IServiceCollection, Task> configureRuntimeServices = null)
        {
            this.pluginRuntime = pluginRuntime;
            this.configureRuntimeServices = configureRuntimeServices;
        }

        protected event EventHandler<IServiceProvider>? OnValidating;

        protected event EventHandler<IServiceProvider>? OnMutating;

        protected event EventHandler<IServiceProvider>? OnFinalize;

        protected event EventHandler<IServiceProvider>? OnNotify;

        public async Task<PlatformResponse> ExecuteAsync(PlatformRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Capture exceptions and return a PlatformResponse with the exception details
            try
            {
                IInternalRuntimeExecutionContext context = request.ToExecutionContext(cancellationToken);
                await this.ExecuteAsync(context);
                return context.ToPlatformResponse();
            }
            // TODO: Handle exceptions entirely different and only use them for exceptional situations!
            catch (PlatformRuntimeException ex)
            {
                return new PlatformResponse
                {
                    Message = request.Message,
                    ActivityId = request.ActivityId,
                    Status =
                    {
                        Message = ex.Message,
                        Code = ex.HttpResponseCode,
                    },
                };
            }
            catch (Exception ex)
            {
                throw new PlatformRuntimeException("Unhandled runtime exception.", ex);
            }
        }

        private async Task ExecuteAsync(IInternalRuntimeExecutionContext context)
        {
            IServiceCollection descriptors = new ServiceCollection();
            descriptors.AddSingleton<IRuntimeExecutionContext>(context);

            if (this.configureRuntimeServices != null)
            {
                await this.configureRuntimeServices.Invoke(descriptors);
            }

            using ServiceProvider services = descriptors.BuildServiceProvider();
            using IServiceScope scope = services.CreateScope();

            // TODO: Use transacted stores ...
            context.SetStep(OperationStep.PreOperation);
            await this.pluginRuntime.ExecuteAsync(scope.ServiceProvider);

            context.SetStep(OperationStep.Operation);
            await this.pluginRuntime.ExecuteAsync(scope.ServiceProvider);

            context.SetStep(OperationStep.PostOperation);
            await this.pluginRuntime.ExecuteAsync(scope.ServiceProvider);
        }
    }
}
