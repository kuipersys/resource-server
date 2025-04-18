﻿// <copyright file="IKuiperBuilder.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Abstractions
{
    public interface IResourceDefinitionManagerBuilder
    {
        IResourceDefinitionManagerBuilder RegisterResourceDefinition<TResourceDefinition>(TResourceDefinition resourceDefinition)
            where TResourceDefinition : class, new();
    }
}
