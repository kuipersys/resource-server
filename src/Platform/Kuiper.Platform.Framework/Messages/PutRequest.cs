// <copyright file="PutRequest.cs" company="Kuiper Microsystems, LLC">
// Copyright (c) Kuiper Microsystems, LLC. All rights reserved.
// </copyright>

namespace Kuiper.Platform.Framework.Messages
{
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework;

    public sealed class PutRequest : PlatformRequest
    {
        public PutRequest()
            : base(Constants.Put)
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

        [JsonIgnore]
        public bool? Overwrite
        {
            get => this.InputParameters.Get<bool>(ConvertName(nameof(this.Overwrite)));
            set => this.InputParameters.Set(ConvertName(nameof(this.Overwrite)), value);
        }
    }
}
