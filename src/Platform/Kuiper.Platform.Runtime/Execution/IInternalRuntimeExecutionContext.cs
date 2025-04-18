// <copyright file="IInternalExecutionContext.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution
{
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Extensibility;

    [JsonDerivedType(typeof(PlatformRuntimeExecutionContext), nameof(PlatformRuntimeExecutionContext))]
    public interface IInternalRuntimeExecutionContext : IRuntimeExecutionContext
    {
        public PlatformResponse ToPlatformResponse();

        public void SetStep(OperationStep step);
    }
}
