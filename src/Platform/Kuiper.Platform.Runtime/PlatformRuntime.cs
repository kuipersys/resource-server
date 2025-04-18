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
        private readonly Func<IServiceCollection, Task> configureExecutionServices;

        public PlatformRuntime(IPluginRuntime pluginRuntime, Func<IServiceCollection, Task> configureExecutionServices = null)
        {
            this.pluginRuntime = pluginRuntime;
            this.configureExecutionServices = configureExecutionServices;
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
                    RequestId = request.RequestId,
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

            if (this.configureExecutionServices != null)
            {
                await this.configureExecutionServices.Invoke(descriptors);
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
