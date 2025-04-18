// <copyright file="GetCommand.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.CommandHandlers
{
    using Kuiper.Platform.Framework.Errors;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Runtime.Abstractions.Command;
    using Kuiper.Platform.Runtime.Command;
    using Kuiper.ServiceInfra.Abstractions.Persistence;

    using Microsoft.Extensions.DependencyInjection;

    [Command(verb: "get")]
    public class GetResourceCommandHandler : ICommandHandler
    {
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context)
        {
            await Task.CompletedTask;

            var store = context.Services.GetRequiredService<IKeyValueStore>();

            try
            {
                var data = await store.GetAsync($"{context.Command.NamedArguments["namespace"]}/{context.Command.NamedArguments["group"]}/{context.Command.NamedArguments["kind"]}/{context.Command.NamedArguments["name"]}", context.CancellationToken);

                if (data == null)
                {
                    throw new PlatformException(PlatformErrorCodes.ResourceNotFound);
                }

                return CommandResult.Success(data);
            }
            catch (KeyNotFoundException ex)
            {
                return CommandResult.Failure("ResourceNotFound", statusCode: 404);
            }
        }
    }
}
