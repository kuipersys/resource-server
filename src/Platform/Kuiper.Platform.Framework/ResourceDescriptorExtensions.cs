// <copyright file="ResourceDescriptorExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework
{
    public static class ResourceDescriptorExtensions
    {
        public static ResourceDescriptor AsResourceDescriptor(this SystemObject systemObject)
        {
            systemObject.NormalizeResource();

            var apiVersionParts = systemObject.ApiVersion.Split('/');
            var group = apiVersionParts.Length > 1 ? apiVersionParts.First() : Constants.Resources.SYSTEM_GROUP;
            var version = apiVersionParts.Last();

            return new ResourceDescriptor()
            {
                Group = group,
                GroupVersion = version,
                Namespace = systemObject.Metadata.Namespace ?? Constants.Resources.DEFAULT_NAMESPACE,
                Kind = systemObject.Kind,
                Name = systemObject.Metadata.Name,
            };
        }

        public static SystemObject NormalizeResource(this SystemObject systemObject)
            => systemObject
                .NormalizeResourceApiVersion()
                .NormalizeResourceNamespace()
                .NormalizeResourceKind()
                .NormalizeResourceName()
                .NormalizeResourceVersion()
                .NormalizeResourceUid()
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

        public static SystemObject NormalizeResourceUid(this SystemObject systemObject)
        {
            if (string.IsNullOrWhiteSpace(systemObject.Metadata.Uid))
            {
                return systemObject;
            }

            systemObject.Metadata.Uid = systemObject.Metadata.Uid.ToLower();

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

        public static SystemObject NormalizeResourceKind(this SystemObject systemObject)
        {
            // We don't need to normalize the kind here since we override it during the create
            // proces with what's defined in the resource definition.

            // Also - we lower case in the resource definition resource manager so that
            // lookups are case insensitive.

            return systemObject;
        }

        public static SystemObject NormalizeResourceNamespace(this SystemObject systemObject)
        {
            if (string.IsNullOrWhiteSpace(systemObject.Metadata.Namespace))
            {
                systemObject.Metadata.Namespace = Constants.Resources.DEFAULT_NAMESPACE;
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
            var group = apiVersionParts.Length > 1 ? apiVersionParts.First() : Constants.Resources.SYSTEM_GROUP;
            var version = apiVersionParts.Last();

            systemObject.ApiVersion = $"{group}/{version}".ToLower();

            return systemObject;
        }
    }
}
