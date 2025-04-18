// <copyright file="ListResourceCommandHandler.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.CommandHandlers
{
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Abstractions.Command;
    using Kuiper.Platform.Runtime.Command;
    using Kuiper.Platform.Serialization.Serialization;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    using Microsoft.Extensions.DependencyInjection;

    [Command(verb: "list")]
    public class ListResourceCommandHandler : ICommandHandler
    {
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context)
        {
            var store = context.Services.GetRequiredService<IKeyValueStore>();

            var data = await store.ScanValuesAsync($"{context.Command.NamedArguments["namespace"]}/{context.Command.NamedArguments["group"]}/{context.Command.NamedArguments["kind"]}", context.CancellationToken);

            if (data == null)
            {
                return CommandResult.Failure("No data found");
            }

            var result = new List<SystemObject>();

            foreach (var item in data)
            {
                var systemObject = item.Value.JsonBytesToObject<SystemObject>();

                if (systemObject != null)
                {
                    result.Add(systemObject);
                }
            }

            return CommandResult.Success(result);
        }
    }
}
