// <copyright file="ReadyStatus.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Checks
{
    public enum ReadyStatus
    {
        /// <summary>
        /// The system is in an unknown state.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The system is not ready to accept requests.
        /// </summary>
        Offline = 10,

        /// <summary>
        /// The system is in the process of initializing.
        /// </summary>
        Initializing = 20,

        /// <summary>
        /// The system is in the process of recovering from a failure.
        /// </summary>
        Recovering = 30,

        /// <summary>
        /// The system is ready to accept requests.
        /// </summary>
        Online = 40,

        /// <summary>
        /// The system is in a failed state, the next step is to restart the system
        /// or to take move it to Offline and through the recovery process.
        /// </summary>
        Failed = 50,
    }
}
