// <copyright file="StateTransition.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.StateMachine
{
    internal class StateTransition<TCommand, TState>
        where TCommand : notnull, Enum
        where TState : notnull, Enum
    {
        internal readonly TCommand Command;
        internal readonly TState CurrentState;
        internal readonly TState NewState;

        public StateTransition(TCommand command, TState currentState, TState newState)
        {
            this.Command = command;
            this.CurrentState = currentState;
            this.NewState = newState;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.Command.GetHashCode(),
                this.CurrentState.GetHashCode(),
                this.NewState.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            StateTransition<TCommand, TState>? other = obj as StateTransition<TCommand, TState>;

            return other != null &&
                EqualityComparer<TState>.Default.Equals(this.CurrentState, other.CurrentState) &&
                EqualityComparer<TCommand>.Default.Equals(this.Command, other.Command) &&
                EqualityComparer<TState>.Default.Equals(this.NewState, other.NewState);
        }

        public bool CanTransition(TCommand command, TState? currentState)
        {
            return
                currentState != null &&
                EqualityComparer<TCommand>.Default.Equals(this.Command, command) &&
                EqualityComparer<TState>.Default.Equals(this.CurrentState, currentState);
        }
    }
}
