using System.Threading.Tasks;

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    public interface ICommandHandler
    {
        Task<ICommandResult> ExecuteAsync(ICommandContext context);
    }
}
