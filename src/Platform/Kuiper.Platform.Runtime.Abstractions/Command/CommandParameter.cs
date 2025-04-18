// <copyright file="CommandArgument.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    using System;

    public class CommandParameter
    {
        public string Name { get; }

        public Type Type { get; }

        public bool IsRequired { get; }

        public object? DefaultValue { get; }

        public CommandParameter(string name, Type type, bool isRequired, object? defaultValue = null)
        {
            this.Name = name;
            this.Type = type;
            this.IsRequired = isRequired;
            this.DefaultValue = defaultValue;
        }
    }
}
