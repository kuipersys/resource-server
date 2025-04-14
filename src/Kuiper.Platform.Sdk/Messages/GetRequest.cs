// <copyright file="GetRequest.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk.Messages
{
    using System.Text.Json.Serialization;

    public class GetRequest : PlatformRequest
    {
        public GetRequest()
            : base(Constants.Get)
        {
        }

        [JsonIgnore]
        public ResourceDescriptor? Target
        {
            get => this.InputParameters.Get<ResourceDescriptor>(ConvertName(nameof(this.Target)));
            set => this.InputParameters.Set(ConvertName(nameof(this.Target)), value);
        }
    }
}
