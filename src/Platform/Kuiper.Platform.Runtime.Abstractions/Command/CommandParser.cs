// <copyright file="CommandParser.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Abstractions.Command
{
    using System;

    public static class SimpleCommandParser
    {
        public static ParsedCommand Parse(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ArgumentException("No command provided.");
            }

            var result = new ParsedCommand
            {
                CommandName = args[0],
            };

            string? lastFlag = null;

            for (int i = 1; i < args.Length; i++)
            {
                var token = args[i];

                if (token.StartsWith("--", StringComparison.OrdinalIgnoreCase))
                {
                    // It's a named argument or flag
                    lastFlag = token[2..]; // remove the --

                    // Peek to see if next token is a value or another flag
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.OrdinalIgnoreCase))
                    {
                        result.NamedArguments[lastFlag] = args[i + 1];
                        i++; // consume the value
                    }
                    else
                    {
                        // flag-style arg (e.g. --verbose)
                        result.NamedArguments[lastFlag] = "true";
                    }
                }
                else
                {
                    // If we didn't just see a flag, it's positional
                    result.PositionalArguments.Add(token);
                }
            }

            return result;
        }
    }

}
