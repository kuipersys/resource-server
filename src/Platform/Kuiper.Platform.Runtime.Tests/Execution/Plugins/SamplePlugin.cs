// <copyright file="SamplePlugin.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.Execution.Plugins
{
    using System;
    using System.Threading.Tasks;

    using Kuiper.Platform.Framework.Extensibility;
    using Kuiper.Platform.Runtime.Execution.Attributes;

    /// <summary>
    /// Defines the <see cref="SamplePlugin" />
    /// </summary>
    [RequiredOutput("WasExecuted", typeof(bool))]
    [RequiredSharedVariable(ExecutionPhase.Post, "iterations", typeof(int))]
    public class SamplePlugin : IPlugin
    {
        public Task ExecuteAsync(IServiceProvider serviceProvider)
        {
            var executionContext = (IExecutionContext)serviceProvider.GetService(typeof(IExecutionContext));
            executionContext.OutputParameters["WasExecuted"] = true;

            if (executionContext.SharedVariables.ContainsKey("iterations"))
            {
                executionContext.SharedVariables["iterations"] = (int)executionContext.SharedVariables["iterations"] + 1;
            }
            else
            {
                executionContext.SharedVariables["iterations"] = 1;
            }

            return Task.CompletedTask;
        }
    }
}
