// <copyright file="IResourceRequestValidator.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Service.Core
{
    using Kuiper.Platform.Framework;

    public interface IResourceRequestValidator
    {
        Task ValidateAsync(ResourceDescriptor? resourceDescriptor, bool throwOnNull = true);
        Task ValidateAsync(SystemObject systemObject);
    }
}
