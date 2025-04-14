// <copyright file="YamlDotNetExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Utilities.Serialization
{
    using Kuiper.Platform.Utilities.Serialization.Yaml;

    using YamlDotNet.Serialization;

    public static class YamlDotNetExtensions
    {
        public static string ObjectToYaml<T>(this T value)
        {
            var serializer = new SerializerBuilder()
                .WithTypeConverter(new JsonElementYamlTypeConverter())
                .WithTypeInspector(x => new SortedTypeInspector(x))
                .Build();

            var rawValue = value
                .ObjectToJson()
                .ObjectFromJson<object>();

            return serializer.Serialize(rawValue);
        }

        public static T ObjectFromYaml<T>(this string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .Build();

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var rawObject = deserializer
                .Deserialize(yaml);

            var rawJson = serializer.Serialize(rawObject);

            return rawJson
                .ObjectFromJson<T>();
        }

        public static string YamlToJson(this string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .Build();

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var rawObject = deserializer
                .Deserialize(yaml);

            var rawJson = serializer.Serialize(rawObject);

            return rawJson;
        }
    }
}
