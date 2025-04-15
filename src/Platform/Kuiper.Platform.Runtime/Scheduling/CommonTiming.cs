// <copyright file="CommonTiming.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Scheduling
{
    public static class CommonTiming
    {
        private static volatile Random Random = new Random();

        public const int MaxTimeoutMs = 350;

        public const int MinTimeoutMs = 100;

        public const int HeartbeatMs = 25;

        public const long HeartbeatTicks = HeartbeatMs * TimeSpan.TicksPerMillisecond;

        public const long MinTimeoutTicks = MinTimeoutMs * TimeSpan.TicksPerMillisecond;

        public const long MaxTimeoutTicks = MaxTimeoutMs * TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// The timeout to wait before an election is started for this <see cref="Server"/>.
        /// </summary>
        public static TimeSpan HeartbeatTimeout
        {
            get
            {
                return new TimeSpan(HeartbeatTicks);
            }
        }

        public static TimeSpan ElectionTimeout
        {
            get
            {
                int timeoutMs = Random.Next(MinTimeoutMs, MaxTimeoutMs);
                return new TimeSpan(timeoutMs * TimeSpan.TicksPerMillisecond);
            }
        }
    }
}
