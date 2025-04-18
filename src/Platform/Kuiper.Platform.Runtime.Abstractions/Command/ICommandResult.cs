// <copyright file="ICommandResult.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    public interface ICommandResult
    {
        bool IsSuccess { get; }

        object? Data { get; }

        string? Message { get; }

        string? ErrorCode { get; }

        int? StatusCode { get; } // optional: 200, 202, 400, etc.
    }
}
