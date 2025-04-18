// <copyright file="IHttpContextCommandFactory.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

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
