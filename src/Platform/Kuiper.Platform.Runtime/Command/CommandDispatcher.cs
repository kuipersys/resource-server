// <copyright file="CommandDispatcher.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Command
{
    using System.Reflection;
    using System.Text;

    using Kuiper.Platform.Runtime.Abstractions.Command;

    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IReadOnlyDictionary<string, ICommandHandler> handlers;

        public CommandDispatcher(IEnumerable<ICommandHandler> registeredHandlers)
        {
            // Key handlers by command name (use DI or reflection for discovery)
            this.handlers = registeredHandlers
                .Where(h => h.GetType().GetCustomAttribute<CommandAttribute>() is null ||
                            !h.GetType().GetCustomAttribute<CommandAttribute>().Disable)
                .ToDictionary(h => this.GetCommandName(h), StringComparer.OrdinalIgnoreCase);
        }

        public async Task<ICommandResult> DispatchAsync(ICommandExecutionContext context)
        {
            if (!this.handlers.TryGetValue(context.CommandName, out var handler))
            {
                return CommandResult.Failure(
                    $"No handler found for command '{context.CommandName}'",
                    errorCode: "HandlerNotFound",
                    statusCode: 404);
            }

            try
            {
                return await handler.ExecuteAsync(context);
            }
            catch (OperationCanceledException)
            {
                return CommandResult.Failure("Command was cancelled.", "Cancelled", 499);
            }
            catch (Exception ex)
            {
                // Log this in real implementation
                return CommandResult.Failure($"Unhandled exception: {ex.Message}", "UnhandledException", 500);
            }
        }

        private string GetCommandName(ICommandHandler handler)
        {
            var handlerType = handler.GetType();
            var commandAttribute = handlerType.GetCustomAttribute<CommandAttribute>();

            if (commandAttribute == null)
            {
                // Could be based on convention, attribute, or an interface property
                var typeName = handlerType.Name;
                return typeName.Replace("CommandHandler", string.Empty); // e.g., CreateResourceCommandHandler => "CreateResource"
            }

            return commandAttribute.CommandId;
        }
    }
}
