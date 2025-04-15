// <copyright file="StateTransitionBuilder.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.StateMachine
{
    internal class StateTransitionBuilder<TCommand, TState> : IStateTransitionBuilder<TCommand, TState>
        where TCommand : notnull, Enum
        where TState : notnull, Enum
    {
        private IList<StateTransition<TCommand, TState>> transitions
            = new List<StateTransition<TCommand, TState>>();

        private StateTransitionBuilder()
        {
        }

        public static IStateTransitionBuilder<TCommand, TState> CreateBuilder()
        {
            return new StateTransitionBuilder<TCommand, TState>();
        }

        public IStateTransitionBuilder<TCommand, TState> AddTransition(TCommand command, TState currentState, TState newState)
        {
            this.transitions.Add(new StateTransition<TCommand, TState>(command, currentState, newState));

            return this;
        }

        public IStateTransitionBuilder<TCommand, TState> AddTransitionsFor(TCommand command, TState newState, params TState[] currentStates)
        {
            foreach (TState currentState in currentStates)
            {
                this.AddTransition(command, currentState, newState);
            }

            return this;
        }

        public IStateTransitionBuilder<TCommand, TState> AddTransitionsExcept(TCommand command, TState newState, params TState[] except)
        {
            // all states except new state
            TState[] currentStates = ((TState[])Enum.GetValues(typeof(TState)))
                .Where(s => !s.Equals(newState))
                .Except(except ?? Array.Empty<TState>())
                .ToArray();

            return this.AddTransitionsFor(command, newState, currentStates);
        }

        public IReadOnlyList<StateTransition<TCommand, TState>> Build()
        {
            this.Validate();

            return this.transitions.ToArray();
        }

        public void Validate()
        {
            TCommand[] commandValues = (TCommand[])Enum.GetValues(typeof(TCommand));
            TState[] stateValues = (TState[])Enum.GetValues(typeof(TState));

            foreach (TCommand command in commandValues)
            {
                foreach (TState state in stateValues)
                {
                    int transitionCount = this.transitions
                        .Where(t => t.CanTransition(command, state))
                        .Count();

                    if (transitionCount > 1)
                    {
                        throw new InvalidOperationException($"Invalid State Transition: {command} -> {state}, the next state transition is non-deterministic.");
                    }
                }
            }
        }
    }
}
