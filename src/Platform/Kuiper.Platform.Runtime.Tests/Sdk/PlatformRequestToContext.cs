// <copyright file="PlatformRequestToContext.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.Sdk
{
    using Kuiper.Platform.Framework.Extensibility;
    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Execution;
    using Kuiper.Platform.Runtime.Extensibility;
    using Kuiper.Platform.Runtime.Tests.Execution.Plugins;
    using Kuiper.Platform.Serialization.Serialization;

    using Shouldly;

    using Snapshooter.Xunit;

    public class PlatformRequestToContext
    {
        [Fact]
        public void Test()
        {
            var request = new PutRequest();

            request.Target = new SystemObject
            {
                ApiVersion = "v1",
            };

            IInternalRuntimeExecutionContext context = request.ToExecutionContext();
            string json = context.ObjectToJson(typeof(IInternalRuntimeExecutionContext), true);
            json.MatchSnapshot();

            IInternalRuntimeExecutionContext result = json.ObjectFromJson<IInternalRuntimeExecutionContext>();
            result.ShouldBeOfType<PlatformRuntimeExecutionContext>();
        }

        [Fact]
        public async Task OperationEngineOk()
        {
            int iterations = 2000;
            PluginManager pluginManager = new PluginManager();
            PluginRuntime pluginRuntime = new PluginRuntime(pluginManager);

            pluginManager.Register<SamplePlugin>("Put", OperationStep.PreOperation);

            var request = new PutRequest();

            request.Target = new SystemObject
            {
                ApiVersion = "v1",
            };

            var engine = new PlatformRuntime(pluginRuntime);

            for (int i = 0; i < iterations; i++)
            {
                _ = await engine.ExecuteAsync(request);
            }

            var result = await engine.ExecuteAsync(request);
            string json = result.ObjectToJson(true);
            json.MatchSnapshot();
        }
    }
}
