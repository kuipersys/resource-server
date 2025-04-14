// <copyright file="StateChangeNotification.cs" company="Kuiper Microsystems, LLC">
// Copyright (c) Kuiper Microsystems, LLC. All rights reserved.
// </copyright>

namespace Kuiper.Platform.Runtime.StateMachine
{
    public delegate void StateChangeNotification<TState>(TState oldState, TState newState)
        where TState : notnull, Enum;
}
