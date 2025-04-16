// <copyright file="ResourcePathParser.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using Kuiper.Platform.Framework;

    public static class ResourcePathParser
    {
        public static bool TryParse(string path, out ResourceDescriptor? resourceDescriptor)
        {
            try
            {
                resourceDescriptor = Parse(path);
                return true;
            }
            catch (Exception)
            {
                resourceDescriptor = null;
                return false;
            }
        }

        public static ResourceDescriptor Parse(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentException("Full path cannot be null or empty.", nameof(fullPath));
            }

            var segments = fullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 4)
            {
                throw new ArgumentException("Full path must have at least 2 segments: group, api version, namespace, and resourceType.");
            }

            var group = segments[0];
            var @namespace = segments[1];
            var resourceType = segments[2];
            var resourceName = segments.Length > 3 ? segments[3] : null;
            var subResourcePath = segments.Length > 4 ? string.Join('/', segments, 4, segments.Length - 4) : null;

            return new ResourceDescriptor()
            {
                Group = group,
                // API VERSION IS NOT INCLUDED IN THE URI FOR RESOURCES:
                // GroupVersion = apiVersion,
                Namespace = @namespace,
                Kind = resourceType,
                Name = resourceName,
            };
        }
    }
}
