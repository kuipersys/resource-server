// <copyright file="IKuiperBuilder.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using Kuiper.Platform.Framework.Abstractions.Objects;

namespace Kuiper.Platform.Framework.Abstractions
{
    public interface IResourceDefinitionManagerBuilder
    {
        IResourceDefinitionManagerBuilder RegisterResourceDefinition<TResource>(TResource resource)
            where TResource : IResourceDefinition;
    }
}
