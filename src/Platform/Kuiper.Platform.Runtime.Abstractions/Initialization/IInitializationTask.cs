// <copyright file="IInitializationTask.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Initialization
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInitializationTask
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
