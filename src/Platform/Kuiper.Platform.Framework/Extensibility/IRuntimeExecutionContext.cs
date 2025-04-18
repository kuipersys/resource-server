// <copyright file="IExecutionContext.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Extensibility
{
    public interface IRuntimeExecutionContext
    {
        /// <summary>
        /// Indicates message name executing in the pipeline.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Indicates the step in which the operation is executing.
        /// </summary>
        OperationStep Step { get; }

        /// <summary>
        /// If the execution is canceled this token will indicate the cancellation.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// This identifier will allow us to correlate an execution in logging.
        /// </summary>
        Guid ActivityId { get; }

        /// <summary>
        /// Input parameters in an execution request.
        /// </summary>
        IDictionary<string, object> InputParameters { get; }

        /// <summary>
        /// Output parameters in an execution request.
        /// </summary>
        IDictionary<string, object> OutputParameters { get; }

        /// <summary>
        /// The parent execution context in the event this is nested.
        /// </summary>
        IRuntimeExecutionContext? ParentContext { get; }

        /// <summary>
        /// Variables shared between steps. This is not cleared when the swapInputAfterEachStep parameter is set on the main pipeline.
        /// </summary>
        IDictionary<string, object> SharedVariables { get; }
    }
}
