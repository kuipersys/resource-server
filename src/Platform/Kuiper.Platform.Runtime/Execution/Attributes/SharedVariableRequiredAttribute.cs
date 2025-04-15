// <copyright file="SharedVariableRequiredAttribute.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution.Attributes
{
    using System;

    using Kuiper.Platform.Extensions.Contracts;
    using Kuiper.Platform.Framework.Extensibility;

    public class RequiredSharedVariableAttribute : PluginAssertionAttribute
    {
        private readonly string variableName;

        private readonly Type variableType;

        public RequiredSharedVariableAttribute(ExecutionPhase phase, string variableName, Type variableType)
            : base(phase)
        {
            this.variableName = variableName;
            this.variableType = variableType;
        }

        public override void Assert(IExecutionContext context)
        {
            context.SharedVariables.ThrowIfKeyNotFound(this.variableName);

            context.SharedVariables[this.variableName]
                .ThrowIfNotAssignableTo(this.variableType, new InvalidCastException($"Cannot Cast SharedVariable['{this.variableName}']`{context.SharedVariables[this.variableName]?.GetType()}` to {this.variableType.FullName}."));
        }
    }
}
