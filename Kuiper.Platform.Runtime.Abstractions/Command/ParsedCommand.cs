using System.Collections.Generic;

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    /// <summary>
    /// TODO: Make it so this is not editable.
    /// </summary>
    public class ParsedCommand
    {
        public string CommandName { get; set; } = string.Empty;
        public List<string> PositionalArguments { get; set; } = new();
        public Dictionary<string, string?> NamedArguments { get; set; } = new();
    }
}
