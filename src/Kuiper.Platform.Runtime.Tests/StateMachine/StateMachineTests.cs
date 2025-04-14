// <copyright file="StateMachineTests.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.StateMachine
{
    public class StateMachineTests
    {
        [Fact]
        public void StateMachine_IsValid()
        {
            // Assert
            var stateMachine = new BasicValidStateMachine();
        }

        [Fact]
        public void StateMachine_IsNotValid()
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() => new BasicInvalidStateMachine());
        }

        [Fact]
        public void StateMachine_HandlerCalled()
        {
            // Arrange
            bool called = false;
            var stateMachine = new BasicValidStateMachine();
            stateMachine.StateChanged += (oldState, newState) =>
            {
                called = true;

                // Assert
                Assert.Equal(BasicState.State1, oldState);
                Assert.Equal(BasicState.State2, newState);
            };

            // Act
            stateMachine.HandleCommand(BasicCommand.Command1);

            // Assert
            Assert.True(called);
        }
    }
}