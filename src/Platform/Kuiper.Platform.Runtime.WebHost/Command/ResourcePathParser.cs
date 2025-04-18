// <copyright file="ResourcePathParser.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.WebHost.Command
{
    using System;

    using Kuiper.Platform.ManagementObjects;

    using Microsoft.AspNetCore.Http;

    public static class ResourcePathParser
    {
        public static bool TryParse(PathString path, out ResourceDescriptor? resourceDescriptor)
            => TryParse(path.ToString(), out resourceDescriptor);

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

        public static ResourceDescriptor Parse(PathString path)
         => Parse(path.ToString());

        public static ResourceDescriptor Parse(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Full path cannot be null or empty.", nameof(path));
            }

            var segments = path.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 3)
            {
                throw new ArgumentException("Full path must have at least 3 segments: group, namespace, and resource kind.");
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
