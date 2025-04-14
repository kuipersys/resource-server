// <copyright file="PatchRequest.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk.Messages
{
    using System.Text.Json.Serialization;

    public class PatchRequest : PlatformRequest
    {
        public PatchRequest()
            : base(Constants.Patch)
        {
        }

        [JsonIgnore]
        public SystemObject? Target
        {
            get => this.InputParameters.Get<SystemObject>(ConvertName(nameof(this.Target)));
            set => this.InputParameters.Set(ConvertName(nameof(this.Target)), value);
        }

        [JsonIgnore]
        public bool? ReturnResult
        {
            get => this.InputParameters.Get<bool>(ConvertName(nameof(this.ReturnResult)));
            set => this.InputParameters.Set(ConvertName(nameof(this.ReturnResult)), value);
        }
    }
}
