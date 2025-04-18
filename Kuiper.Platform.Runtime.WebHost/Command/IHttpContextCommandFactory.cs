namespace Kuiper.Platform.Runtime.WebHost.Command
{
    using System.Threading.Tasks;

    using Kuiper.Platform.Runtime.Abstractions.Command;

    using Microsoft.AspNetCore.Http;

    public interface IHttpContextCommandFactory
    {
        /// <summary>
        /// Creates a command context from the given HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP request context.</param>
        /// <returns>An instance of <see cref="ICommandContext"/> representing the command derived from the request.</returns>
        Task<ICommandContext?> CreateAsync(HttpContext httpContext);
    }
}
