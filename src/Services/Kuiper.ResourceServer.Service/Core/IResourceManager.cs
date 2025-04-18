﻿// <copyright file="IResourceManager.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using Kuiper.Platform.ManagementObjects.v1alpha1;

    public interface IResourceManager
    {
        Task<bool> ResourceVersionExists(string group, string kind, string groupVersion);

        Task<ResourceDefinitionVersion?> GetResourceVersionAsync(string group, string kind, string groupVersion);

        Task<ResourceDefinition?> GetResourceDefinitionAsync(string group, string kind);
    }
}
