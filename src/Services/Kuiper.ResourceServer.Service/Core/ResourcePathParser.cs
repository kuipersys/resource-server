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
            var apiVersion = segments[1];
            var @namespace = segments[2];
            var resourceType = segments[3];
            var resourceName = segments.Length > 4 ? segments[4] : null;
            var subResourcePath = segments.Length > 5 ? string.Join('/', segments, 5, segments.Length - 5) : null;

            return new ResourceDescriptor()
            {
                Group = group,
                GroupVersion = apiVersion,
                Namespace = @namespace,
                Kind = resourceType,
                Name = resourceName,
            };
        }
    }
}
