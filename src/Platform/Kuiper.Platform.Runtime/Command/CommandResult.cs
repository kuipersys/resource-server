// <copyright file="CommandResult.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Command
{
    using Kuiper.Platform.Runtime.Abstractions.Command;

    public class CommandResult : ICommandResult
    {
        public bool IsSuccess { get; private set; }

        public object? Data { get; private set; }

        public string? Message { get; private set; }

        public string? ErrorCode { get; private set; }

        public int? StatusCode { get; private set; }

        private CommandResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public static CommandResult Success(object? data = null, string? message = null, int? statusCode = 200)
        {
            return new CommandResult(true)
            {
                Data = data,
                Message = message,
                StatusCode = statusCode,
            };
        }

        public static CommandResult Accepted(string? message = null)
        {
            return new CommandResult(true)
            {
                Message = message,
                StatusCode = 202,
            };
        }

        public static CommandResult Failure(string message, string? errorCode = null, int? statusCode = 400)
        {
            return new CommandResult(false)
            {
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode,
            };
        }

        public override string ToString()
        {
            return this.IsSuccess
                ? $"Success [{this.StatusCode}]: {this.Message ?? "OK"}"
                : $"Failure [{this.StatusCode}]: {this.ErrorCode ?? "UnknownError"} - {this.Message}";
        }
    }
}
