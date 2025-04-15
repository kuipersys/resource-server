// <copyright file="RequiredOutputAttribute.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution.Attributes
{
    using System;

    using Kuiper.Platform.Extensions.Contracts;
    using Kuiper.Platform.Framework.Extensibility;

    public class RequiredOutputAttribute : PluginAssertionAttribute
    {
        private readonly string variableName;

        private readonly Type variableType;

        private readonly string errorMessage;

        public RequiredOutputAttribute(string variableName, Type variableType)
            : base(ExecutionPhase.Post)
        {
            this.variableName = variableName;
            this.variableType = variableType;
            this.errorMessage = $"Cannot cast OutputParameter['{this.variableName}'] to {this.variableType.FullName}.";
        }

        public override void Assert(IExecutionContext context)
        {
            context.OutputParameters.ThrowIfKeyNotFound(this.variableName);
            context.OutputParameters[this.variableName]
                .ThrowIfNotAssignableTo(this.variableType, new InvalidCastException(this.errorMessage));
        }
    }
}
