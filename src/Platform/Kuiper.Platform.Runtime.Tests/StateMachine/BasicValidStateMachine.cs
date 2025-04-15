// <copyright file="BasicValidStateMachine.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.StateMachine
{
    using Kuiper.Platform.Runtime.StateMachine;

    internal enum BasicCommand
    {
        Command1,
        Command2,
        Command3,
    }

    internal enum BasicState
    {
        State1,
        State2,
        State3,
    }

    internal class BasicValidStateMachine : StateMachineBase<BasicCommand, BasicState>
    {
        protected override BasicState DefaultState => default;

        protected override void BuildStateMachine(IStateTransitionBuilder<BasicCommand, BasicState> builder)
        {
            builder.AddTransition(BasicCommand.Command1, BasicState.State1, BasicState.State2);
            builder.AddTransition(BasicCommand.Command1, BasicState.State2, BasicState.State1);
            builder.AddTransition(BasicCommand.Command2, BasicState.State2, BasicState.State3);
            builder.AddTransition(BasicCommand.Command3, BasicState.State3, BasicState.State1);
        }
    }

    internal class BasicInvalidStateMachine : StateMachineBase<BasicCommand, BasicState>
    {
        protected override BasicState DefaultState => default;

        protected override void BuildStateMachine(IStateTransitionBuilder<BasicCommand, BasicState> builder)
        {
            // A command is essentially a function that takes a state and returns a new state.
            // A single command, given a state, should always return the same new state.
            // A command cannot return multiple new states for a single state, but it can return
            // a differnt state given a different input state.
            builder.AddTransition(BasicCommand.Command1, BasicState.State1, BasicState.State2);
            builder.AddTransition(BasicCommand.Command1, BasicState.State1, BasicState.State3);
        }
    }
}
