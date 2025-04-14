// <copyright file="RequiredInputAttribute.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution.Attributes
{
    using System;

    using Kuiper.Platform.Extensions;
    using Kuiper.Platform.Sdk.Extensibility;

    public class RequiredInputAttribute : PluginAssertionAttribute
    {
        private readonly string variableName;

        private readonly Type variableType;

        private readonly string errorMessage;

        public RequiredInputAttribute(string variableName, Type variableType)
            : base(ExecutionPhase.Pre)
        {
            this.variableName = variableName;
            this.variableType = variableType;
            this.errorMessage = $"Cannot cast InputParameter['{this.variableName}'] to {this.variableType.FullName}.";
        }

        public override void Assert(IExecutionContext context)
        {
            context.InputParameters.ThrowIfKeyNotFound(this.variableName);
            context.InputParameters[this.variableName]
                .ThrowIfNotAssignableTo(this.variableType, new InvalidCastException(this.errorMessage));
        }
    }
}
