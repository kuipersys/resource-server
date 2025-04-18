using System;

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    public class CommandParameter
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsRequired { get; }
        public object? DefaultValue { get; }
        public CommandParameter(string name, Type type, bool isRequired, object? defaultValue = null)
        {
            Name = name;
            Type = type;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }
    }
}
