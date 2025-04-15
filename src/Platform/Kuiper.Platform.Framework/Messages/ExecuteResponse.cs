// <copyright file="ExecuteResponse.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Messages
{
    public class ExecuteResponse : PlatformResponse
    {
        public ExecuteResponse()
            : base(Constants.Execute)
        {
        }

        public ExecuteResponse(string message)
            : base(message)
        {
        }
    }
}
