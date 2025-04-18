// <copyright file="ICommandContext.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public interface ICommandContext
    {
        string CommandName { get; }

        object? Payload { get; }

        string? ActivityId { get; }

        string? Source { get; } // e.g., "HTTP", "CLI", "ResourceManager"

        DateTime Timestamp { get; }

        CancellationToken CancellationToken { get; }

        ParsedCommand Command { get; }

        IDictionary<string, object> Metadata { get; }

        IServiceProvider Services { get; }

        public T? GetPayload<T>()
            where T : class;
    }
}
