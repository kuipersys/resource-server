namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        public string Module { get; } = string.Empty;

        public string Resource { get; }

        public string Verb { get; }

        public string? Description { get; }

        public bool Disable { get; } = true;

        public string CommandId { get; }

        public CommandAttribute(string verb, string? description = null, string? module = null, string? resource = null, bool disable = false)
        {
            Verb = verb.ToLowerInvariant();
            Description = description;
            Disable = disable;

            if (string.IsNullOrWhiteSpace(verb) ||
                !IsValidCommandName(verb))
            {
                throw new Exception("Command Verbs can only contain letters and numbers.");
            }

            if (module != null && (string.IsNullOrWhiteSpace(module) || !IsValidCommandName(module)))
            {
                throw new Exception("Module Names can only contain letters and numbers.");
            }

            if (resource != null && (string.IsNullOrWhiteSpace(resource) || !IsValidCommandName(resource)))
            {
                throw new Exception("Resource Kinds can only contain letters and numbers.");
            }

            Module = (module ?? string.Empty).ToLowerInvariant();
            Resource = (resource ?? string.Empty).ToLowerInvariant();

            CommandId = this.AsCommandIdentifier();
        }

        public static bool IsValidCommandName(string commandName)
            => Regex.IsMatch(commandName, "^[a-z0-9-]+$");

        private string AsCommandIdentifier()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Resource))
            {
                sb.Append(Resource);
                sb.Append('.');
            }

            if (!string.IsNullOrWhiteSpace(Module))
            {
                sb.Append(Module);
                sb.Append('.');
            }

            sb.Append(Verb);

            return sb.ToString();
        }
    }
}
