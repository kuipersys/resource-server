namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    using System.Threading.Tasks;

    public interface ICommandDispatcher
    {
        Task<ICommandResult> DispatchAsync(ICommandContext context);
    }
}
