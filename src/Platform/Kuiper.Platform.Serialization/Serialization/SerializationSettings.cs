// <copyright file="Settings.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Serialization.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Serialization.Serialization.Json;

    public static class SerializationSettings
    {
        static SerializationSettings()
        {
            Converters.Add(new JsonStringEnumConverter());
            Converters.Add(new JsonBoolConverter());
            Converters.Add(new SystemTypeConverter());

            UpdateSerializationOptions();
        }

        public static HashSet<JsonConverter> Converters { get; private set; } = new HashSet<JsonConverter>();

        public static JsonSerializerOptions IndentedWriteOptions { get; private set; } = CreateJsonSerializerOptions(true, true);

        public static JsonSerializerOptions StandardWriteOptions { get; private set; } = CreateJsonSerializerOptions(false, true);

        public static JsonSerializerOptions StandardReadOptions { get; private set; } = CreateJsonSerializerOptions(false, false);

        public static void AddConverter(JsonConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            Converters.Add(converter);

            UpdateSerializationOptions();
        }

        private static JsonSerializerOptions CreateJsonSerializerOptions(bool writeIndented, bool forSerialization)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = writeIndented,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreReadOnlyProperties = true,
                IgnoreReadOnlyFields = true,
                IncludeFields = true,
                MaxDepth = 25,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };

            foreach (var converter in Converters)
            {
                if (converter is JsonConverter jsonConverter)
                {
                    options.Converters.Add(jsonConverter);
                }
            }

            return options;
        }

        private static void UpdateSerializationOptions()
        {
            IndentedWriteOptions = CreateJsonSerializerOptions(true, true);
            StandardWriteOptions = CreateJsonSerializerOptions(false, true);
            StandardReadOptions = CreateJsonSerializerOptions(false, false);
        }
    }
}
