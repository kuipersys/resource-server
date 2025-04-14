// <copyright file="PluginTests.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.Execution
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Execution;
    using Kuiper.Platform.Runtime.Extensibility;
    using Kuiper.Platform.Runtime.Tests.Execution.Plugins;
    using Kuiper.Platform.Sdk.Extensibility;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    /// <summary>
    /// Defines the <see cref="PluginTests" />
    /// </summary>
    public class PluginTests
    {
        [Fact]
        public async Task PluginManager_ExecuteWithoutServiceProviderAsync()
        {
            // Arrange
            PluginManager pluginManager = new PluginManager();
            PluginRuntime pluginRuntime = new PluginRuntime(pluginManager);

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => pluginRuntime.ExecuteAsync(null));
        }

        [Fact]
        public async Task PluginManager_ValidatePerformance()
        {
            // Arrange
            PluginManager pluginManager = new PluginManager();
            PluginRuntime pluginRuntime = new PluginRuntime(pluginManager);
            int iterations = 20000;

            pluginManager.Register<SamplePlugin>("SomeEntity.DoFoo", OperationStep.PreOperation);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(IExecutionContext), (provider) =>
            {
                return new PlatformRequestExecutionContext("SomeEntity.DoFoo");
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Act
            var context = serviceProvider.GetRequiredService<IExecutionContext>();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < iterations; i++)
            {
                await pluginRuntime.ExecuteAsync(serviceProvider);
            }

            timer.Stop();

            // Assert
            Assert.True(context.OutputParameters.ContainsKey("WasExecuted"));
            Assert.True((bool)context.OutputParameters["WasExecuted"]);
            // 350ms is a reasonable time for 20,000 iterations
            // At this rate - each iteration takes ~0.0175ms
            // the equation for this is 350ms / 20000 iterations = 0.0175ms per iteration
            double nanoSecondsPer = timer.Elapsed.TotalNanoseconds / (double)iterations;

            // 1.0  ms = 1,000,000 ns
            // 0.5  ms =   500,000 ns
            // 0.25 ms =   250,000 ns
            Assert.True(500000 > nanoSecondsPer, $"Elapsed: {nanoSecondsPer}ns");
            Assert.True((int)context.SharedVariables["iterations"] == iterations);
        }

        // [Fact]
        // Wild cards are no longer supported
        public async Task PluginManager_ExecuteWildCardAsync()
        {
            // Arrange
            PluginManager pluginManager = new PluginManager();
            PluginRuntime pluginRuntime = new PluginRuntime(pluginManager);

            pluginManager.Register<SamplePlugin>("SomeEntity.*", OperationStep.PreOperation);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(IExecutionContext), (provider) =>
            {
                return new PlatformRequestExecutionContext("SomeEntity.DoFoo");
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Act
            var context = serviceProvider.GetRequiredService<IExecutionContext>();
            await pluginRuntime.ExecuteAsync(serviceProvider);

            // Assert
            Assert.True(context.OutputParameters.ContainsKey("WasExecuted"));
            Assert.True((bool)context.OutputParameters["WasExecuted"]);
        }

        [Fact]
        public async Task PluginManager_ExecuteWildWildCardPreNotExecutedAsync()
        {
            // Arrange
            PluginManager pluginManager = new PluginManager();
            PluginRuntime pluginRuntime = new PluginRuntime(pluginManager);

            pluginManager.Register<SamplePlugin>("*", OperationStep.PostOperation);
            pluginManager.Register<SamplePlugin>("OtherEntity.*", OperationStep.PreOperation);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(IExecutionContext), (provider) =>
            {
                return new PlatformRequestExecutionContext("SomeEntity.DoFoo.SomeJunk");
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Act
            var context = serviceProvider.GetRequiredService<IExecutionContext>();
            await pluginRuntime.ExecuteAsync(serviceProvider);

            // Assert
            Assert.True(!context.OutputParameters.ContainsKey("WasExecuted"));
        }
    }
}
