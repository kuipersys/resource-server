// <copyright file="ResourceScope.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    public enum ResourceScope
    {
        Unknown,
        Namespaced,
        System // Technically not kubernetes compliant. The closed in kubernetes would be Cluster
    }
}
