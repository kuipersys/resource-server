// <copyright file="CommandContext.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Command
{
    using Kuiper.Platform.Runtime.Abstractions.Command;
    using Kuiper.Platform.Serialization.Serialization;

    internal class CommandContext : ICommandContext
    {
        public CommandContext(
            IServiceProvider services,
            ParsedCommand command,
            object? payload,
            CancellationToken cancellationToken = default,
            string? correlationId = null,
            string? source = null,
            IDictionary<string, object>? metadata = null)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            this.CommandName = command.CommandName ?? throw new ArgumentNullException(nameof(ParsedCommand.CommandName));
            this.Payload = payload;
            this.CancellationToken = cancellationToken;
            this.ActivityId = correlationId;
            this.Source = source;

            if (metadata != null)
            {
                foreach (var pair in metadata)
                    this.Metadata[pair.Key] = pair.Value;
            }
        }

        public string CommandName { get; }

        public object? Payload { get; }

        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        public string? ActivityId { get; set; }

        public string? Source { get; set; }

        public DateTime Timestamp { get; } = DateTime.UtcNow;

        public CancellationToken CancellationToken { get; }

        public ParsedCommand Command { get; }

        public IServiceProvider Services { get; }

        // Optional helper method to get typed payload
        public T? GetPayload<T>()
            where T : class
            => this.Payload?.MarshalAs<T>();
    }
}
