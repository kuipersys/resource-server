// <copyright file="ResourceDescriptor.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework
{
    using System.Text.Json.Serialization;

    [JsonDerivedType(typeof(ResourceDescriptor), nameof(ResourceDescriptor))]
    public class ResourceDescriptor
    {
        private string? @namespace;
        private string? group;
        private string? groupVersion;

        [JsonPropertyOrder(10)]
        public string Namespace
        {
            get
            {
                return this.@namespace ?? Constants.Resources.DEFAULT_NAMESPACE;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.@namespace = Constants.Resources.DEFAULT_NAMESPACE;
                }
                else
                {
                    this.@namespace = value;
                }
            }
        }

        [JsonPropertyOrder(15)]
        public string Group
        {
            get => this.group ?? Constants.Resources.SYSTEM_GROUP;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.group = Constants.Resources.SYSTEM_GROUP;
                }
                else
                {
                    this.group = value;
                }
            }
        }

        [JsonPropertyOrder(16)]
        public string GroupVersion
        {
            get => this.groupVersion ?? Constants.Resources.SYSTEM_VERSION;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.groupVersion = Constants.Resources.SYSTEM_VERSION;
                }
                else
                {
                    this.groupVersion = value;
                }
            }
        }

        [JsonPropertyOrder(20)]
        public string? Kind { get; set; }

        [JsonPropertyOrder(25)]
        public string? Name { get; set; }

        [JsonPropertyOrder(30)]
        public string? Uid { get; set; }

        [JsonPropertyOrder(31)]
        public string? ResourceVersion { get; set; }

        [JsonPropertyOrder(32)]
        public string? Link { get; set; }

        [JsonIgnore]
        public string ApiVersion => $"{this.Group}/{this.GroupVersion}";

        public string ToResourcePrefix()
        {
            IList<string> parts = [];

            if (string.IsNullOrWhiteSpace(this.Kind))
            {
                throw new InvalidOperationException("Resource kind is required to generate resource ID.");
            }

            parts.Add(this.Namespace);
            parts.Add(this.ApiVersion);

            parts.Add(this.Kind);

            return string.Join("/", parts).ToLowerInvariant();
        }

        public string ToResourceId()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new InvalidOperationException("Resource name is required to generate resource ID.");
            }

            return $"{this.ToResourcePrefix()}/{this.Name.ToLowerInvariant()}";
        }
    }
}
