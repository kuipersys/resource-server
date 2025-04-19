// <copyright file="IReadinessCheck.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Checks
{
    using System.Threading.Tasks;

    public interface IReadinessCheck
    {
        public Task<ReadyStatus> CheckAsync();
    }
}
