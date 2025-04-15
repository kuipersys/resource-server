// <copyright file="ConfigurationInvalidException.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

using System;

namespace Kuiper.Platform.Runtime.Execution.Exceptions
{
    public class ConfigurationInvalidException : ConfigurationException
    {
        public ConfigurationInvalidException()
            : base()
        {
        }

        public ConfigurationInvalidException(string message)
            : base(message)
        {
        }

        public ConfigurationInvalidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
