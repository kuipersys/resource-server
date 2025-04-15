// <copyright file="ListResourcePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Plugins
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Errors;
    using Kuiper.Platform.Framework.Extensibility;
    using Kuiper.Platform.Runtime.Execution.Attributes;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    [RequiredInput("target", typeof(ResourceDescriptor))]
    internal class ListResourcePlugin : IPlugin
    {
        public async Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var store = serviceProvider.GetRequiredService<IKeyValueStore>();
            var context = serviceProvider.GetRequiredService<IExecutionContext>();
            ResourceDescriptor resourceDescriptor = context
                .InputParameters["target"]
                .MarshalAs<ResourceDescriptor>();

            if (!string.IsNullOrWhiteSpace(resourceDescriptor.Name))
            {
                // context.Response = new PlatformCommandResponse(400, "Resource Name Is Missing");
                throw new PlatformException("Resource Name Is Not Valid For List");
            }

            try
            {
                var data = await store.ScanValuesAsync(resourceDescriptor.ToResourcePrefix(), context.CancellationToken);

                context.OutputParameters["count"] = data.Count();
                context.OutputParameters["results"] = data
                    .Select(kvp => kvp.Value.JsonBytesToObject<SystemObject>())
                    .ToArray();

                //var response = new PlatformCommandResponse(200);
                //response.SetContent(data);
                //context.Response = response;
            }
            catch (KeyNotFoundException)
            {
                // context.Response = new PlatformCommandResponse(404, "Resource Not Found");
            }
        }
    }
}
