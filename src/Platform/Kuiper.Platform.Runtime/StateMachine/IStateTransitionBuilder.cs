// <copyright file="IStateTransitionBuilder.cs" company="Kuiper Microsystems, LLC">
// Copyright (c) Kuiper Microsystems, LLC. All rights reserved.
// </copyright>

namespace Kuiper.Platform.Runtime.StateMachine
{
    public interface IStateTransitionBuilder<TCommand, TState>
        where TCommand : notnull, Enum
        where TState : notnull, Enum
    {
        IStateTransitionBuilder<TCommand, TState> AddTransition(TCommand command, TState currentState, TState newState);

        IStateTransitionBuilder<TCommand, TState> AddTransitionsFor(TCommand command, TState newState, TState[] currentStates);

        IStateTransitionBuilder<TCommand, TState> AddTransitionsExcept(TCommand command, TState newState, params TState[] except);

        internal IReadOnlyList<StateTransition<TCommand, TState>> Build();

        internal void Validate();
    }
}
