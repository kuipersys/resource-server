// <copyright file="RegisterModuleAttribute.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework.Abstractions
{
    public sealed class RegisterModuleAttribute : Attribute
    {
        public RegisterModuleAttribute(bool enabled = true)
        {
            this.Enabled = enabled;
        }

        public bool Enabled { get; }
    }
}
