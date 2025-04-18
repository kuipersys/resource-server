// <copyright file="PluginAssertionAttribute.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution.Attributes
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Kuiper.Platform.Framework.Extensibility;

    /// <summary>
    /// An abstraction used for pipeline step execution assertions during execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PluginAssertionAttribute : Attribute
    {
        private static readonly IDictionary<ExecutionPhase, IDictionary<Type, IEnumerable<PluginAssertionAttribute>>> assertionAttributes
            = new ConcurrentDictionary<ExecutionPhase, IDictionary<Type, IEnumerable<PluginAssertionAttribute>>>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="step">The step execution phase this given attribute applies to.</param>
        public PluginAssertionAttribute(ExecutionPhase phase)
        {
            this.Phase = phase;
        }

        /// <summary>
        /// Gets step execution step this given attribute applies to.
        /// </summary>
        protected ExecutionPhase Phase { get; }

        /// <summary>
        /// Executes all assertions on a step, synchronously.
        /// </summary>
        /// <param name="phase">The step being executed, used as a filter.</param>
        /// <param name="stepType">The ClrType for the step.</param>
        /// <param name="context">The current execution context.</param>
        /// <exception cref="AggregateException">The aggregation of all failed assertions.</exception>
        public static void ExecuteAssertions(ExecutionPhase phase, Type stepType, IRuntimeExecutionContext context)
        {
            var attributes = GetTypeAssertionAttributes(phase, stepType);

            if (!attributes.Any())
            {
                return;
            }

            var failed = new List<Exception>();

            foreach (var attribute in attributes)
            {
                try
                {
                    attribute.Assert(context);
                }
                catch (Exception ex)
                {
                    failed.Add(ex);
                }
            }

            if (failed.Any())
            {
                throw new AggregateException($"{phase} validation for {stepType.FullName} failed.", failed.ToArray());
            }
        }

        /// <summary>
        /// Executes all assertions on a step, asyncronously.
        /// </summary>
        /// <param name="phase">The phase being executed, used as a filter.</param>
        /// <param name="stepType">The ClrType for the step.</param>
        /// <param name="context">The current execution context.</param>
        /// <returns>An awaitable task.</returns>
        /// <exception cref="AggregateException">The aggregation of all failed assertions.</exception>
        public static Task ExecuteAssertionsAsync(ExecutionPhase phase, Type stepType, IRuntimeExecutionContext context)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    ExecuteAssertions(phase, stepType, context);
                }, context.CancellationToken);
        }

        /// <summary>
        /// The method used in the assertion.
        /// </summary>
        /// <param name="context">The context this assertion applies to.</param>
        public abstract void Assert(IRuntimeExecutionContext context);

        private static IEnumerable<PluginAssertionAttribute> GetTypeAssertionAttributes(ExecutionPhase phase, Type type)
        {
            if (!assertionAttributes.ContainsKey(phase))
            {
                assertionAttributes[phase] = new Dictionary<Type, IEnumerable<PluginAssertionAttribute>>();
            }

            var phaseAssertions = assertionAttributes[phase];

            if (!phaseAssertions.ContainsKey(type))
            {
                phaseAssertions[type] = type.GetCustomAttributes(true)
                    .Where(t => typeof(PluginAssertionAttribute).IsAssignableFrom(t.GetType())).ToArray()
                    .Select(a => a as PluginAssertionAttribute)
                    .Where(a => a.Phase == phase)
                    .ToArray();
            }

            return phaseAssertions[type];
        }
    }
}
