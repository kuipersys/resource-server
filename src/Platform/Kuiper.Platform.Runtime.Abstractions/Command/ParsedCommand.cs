// <copyright file="ParsedCommand.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Make it so this is not editable.
    /// </summary>
    public class ParsedCommand
    {
        public string CommandName { get; set; } = string.Empty;

        public List<string> PositionalArguments { get; set; } = new ();

        public Dictionary<string, string?> NamedArguments { get; set; } = new ();
    }
}
