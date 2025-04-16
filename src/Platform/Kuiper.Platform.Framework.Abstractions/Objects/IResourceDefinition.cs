// <copyright file="Class1.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Abstractions.Objects
{
    public interface IResourceDefinition : ISystemObject
    {
        public string GetKey();

        public IDictionary<string, TVersionObject> GetVersions<TVersionObject>()
            where TVersionObject : class, IResourceDefinitionVersion;
    }
}
