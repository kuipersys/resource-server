// <copyright file="ResourceDescriptorExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using System.Linq;

namespace Kuiper.Platform.ManagementObjects
{
    public static class ResourceDescriptorExtensions
    {
        public static ResourceDescriptor AsResourceDescriptor(this SystemObject systemObject)
        {
            systemObject.NormalizeResource();

            var apiVersionParts = systemObject.ApiVersion.Split('/');
            var group = apiVersionParts.Length > 1 ? apiVersionParts.First() : SystemConstants.Resources.SYSTEM_GROUP;
            var version = apiVersionParts.Last();

            return new ResourceDescriptor()
            {
                Group = group,
                GroupVersion = version,
                Namespace = systemObject.Metadata.Namespace ?? SystemConstants.Resources.DEFAULT_NAMESPACE,
                Kind = systemObject.Kind,
                Name = systemObject.Metadata.Name,
            };
        }

        public static SystemObject NormalizeResource(this SystemObject systemObject)
            => systemObject
                .NormalizeResourceApiVersion()
                .NormalizeResourceNamespace()
                .NormalizeResourceName()
                .NormalizeResourceVersion()
                .NormalizeResourceSelfLink();

        public static SystemObject NormalizeResourceSelfLink(this SystemObject systemObject)
        {
            if (string.IsNullOrWhiteSpace(systemObject.Metadata.SelfLink))
            {
                return systemObject;
            }

            systemObject.Metadata.SelfLink = systemObject.Metadata.SelfLink.ToLower();

            return systemObject;
        }

        public static SystemObject NormalizeResourceVersion(this SystemObject systemObject)
        {
            if (string.IsNullOrWhiteSpace(systemObject.Metadata.ResourceVersion))
            {
                return systemObject;
            }

            systemObject.Metadata.ResourceVersion = systemObject.Metadata.ResourceVersion.ToLower();

            return systemObject;
        }

        public static SystemObject NormalizeResourceName(this SystemObject systemObject)
        {
            if (string.IsNullOrWhiteSpace(systemObject.Metadata.Name))
            {
                return systemObject;
            }

            systemObject.Metadata.Name = systemObject.Metadata.Name.ToLower();

            return systemObject;
        }

        public static SystemObject NormalizeResourceNamespace(this SystemObject systemObject)
        {
            if (string.IsNullOrWhiteSpace(systemObject.Metadata.Namespace))
            {
                systemObject.Metadata.Namespace = SystemConstants.Resources.DEFAULT_NAMESPACE;
            }
            else
            {
                systemObject.Metadata.Namespace = systemObject.Metadata.Namespace.ToLower();
            }

            return systemObject;
        }

        public static SystemObject NormalizeResourceApiVersion(this SystemObject systemObject)
        {
            // Nothin to do here.
            if (string.IsNullOrWhiteSpace(systemObject.ApiVersion))
            {
                return systemObject;
            }

            var apiVersionParts = systemObject.ApiVersion.Split('/');
            var group = apiVersionParts.Length > 1 ? apiVersionParts.First() : SystemConstants.Resources.SYSTEM_GROUP;
            var version = apiVersionParts.Last();

            systemObject.ApiVersion = $"{group}/{version}".ToLower();

            return systemObject;
        }
    }
}
