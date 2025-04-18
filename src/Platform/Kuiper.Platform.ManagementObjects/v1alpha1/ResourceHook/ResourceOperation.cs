// <copyright file="ResourceOperation.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    public enum ResourceOperation
    {
        /// <summary>Triggered when a resource is being created.</summary>
        Create,

        /// <summary>Triggered when a resource is being updated.</summary>
        Update,

        /// <summary>Triggered when a resource is being deleted.</summary>
        Delete,

        /// <summary>Triggered when a resource is being patched.</summary>
        Get,
    }
}
