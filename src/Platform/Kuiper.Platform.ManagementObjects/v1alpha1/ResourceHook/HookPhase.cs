// <copyright file="HookPhase.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    public enum HookPhase
    {
        /// <summary>Runs before the operation is applied. Can mutate the resource.</summary>
        Mutating,

        /// <summary>Runs after mutation, before persistence. Can reject the request.</summary>
        Validating,

        /// <summary>Runs before final deletion, to handle cleanup or prevent orphaned state.</summary>
        Finalizing,

        /// <summary>Runs asynchronously after the operation has been completed successfully.</summary>
        Notifying
    }
}
