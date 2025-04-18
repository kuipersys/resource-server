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
