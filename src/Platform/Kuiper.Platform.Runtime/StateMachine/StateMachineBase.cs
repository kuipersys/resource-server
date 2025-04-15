// <copyright file="StateMachineBase.cs" company="Kuiper Microsystems, LLC">
// Copyright (c) Kuiper Microsystems, LLC. All rights reserved.
// </copyright>

namespace Kuiper.Platform.Runtime.StateMachine
{
    public abstract class StateMachineBase<TCommand, TState>
        where TCommand : struct, Enum
        where TState : struct, Enum
    {
        private readonly object syncLock = new object();
        private readonly IReadOnlyList<StateTransition<TCommand, TState>> transitions;
        private bool disposed = false;

        public StateMachineBase()
        {
            this.CurrentState = this.DefaultState;

            IStateTransitionBuilder<TCommand, TState> builder =
                StateTransitionBuilder<TCommand, TState>.CreateBuilder();

            this.BuildStateMachine(builder);

            this.transitions = builder.Build();
        }

        public event StateChangeNotification<TState>? StateChanged;

        public TState CurrentState { get; private set; }

        protected abstract TState DefaultState { get; }

        public void HandleCommand(TCommand command)
        {
            // Critical Memory Section
            lock (this.syncLock)
            {
                var newState = this.GetNextStateTransition(command);

                if (newState == null)
                {
                    throw new InvalidOperationException($"Invalid command {command} for state {this.CurrentState}");
                }

                if (!EqualityComparer<TState>.Default.Equals(this.CurrentState, newState.Value))
                {
                    var oldState = this.TransitionTo(newState.Value);
                    this.StateChanged?.Invoke(oldState, newState.Value);
                }
            }
        }

        public void Dispose()
        {
            lock (this.syncLock)
            {
                if (this.disposed)
                {
                    return;
                }

                this.Dispose(true);
            }
        }

        protected virtual void Dispose(bool dispose)
        {
        }

        protected abstract void BuildStateMachine(IStateTransitionBuilder<TCommand, TState> builder);

        private TState? GetNextStateTransition(TCommand command)
        {
            var possibleTransitions = this.transitions
                .Where(t => t.CanTransition(command, this.CurrentState))
                .ToList();

            if (!possibleTransitions.Any())
            {
                return null;
            }

            return possibleTransitions
                .Select(t => t.NewState)
                .SingleOrDefault();
        }

        private TState TransitionTo(TState newState)
        {
            TState oldState = this.CurrentState;

            this.CurrentState = newState;

            return oldState;
        }
    }
}
