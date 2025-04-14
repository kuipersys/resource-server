// <copyright file="DeleteResponse.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk.Messages
{
    public class DeleteResponse : PlatformResponse
    {
        public DeleteResponse()
            : base(Constants.Delete)
        {
        }
    }
}
