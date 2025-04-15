// <copyright file="PlatformRequestExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution
{
    using Kuiper.Platform.Framework;

    public static class PlatformRequestExtensions
    {
        public static IInternalExecutionContext ToExecutionContext(this PlatformRequest request, CancellationToken cancellationToken = default)
        {
            return new PlatformRequestExecutionContext(request)
            {
                CancellationToken = cancellationToken,
            };
        }
    }
}
