// <copyright file="SerializationValidation.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.Execution
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.Json.Serialization.Metadata;

    using Snapshooter.Xunit;

    public class SerializationValidation
    {
        //public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
        //{
        //    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        //    {
        //        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        //        Type basePointType = typeof(IExecutionContext);

        //        if (jsonTypeInfo.Type == basePointType)
        //        {
        //            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        //            {
        //                TypeDiscriminatorPropertyName = "$type",
        //                IgnoreUnrecognizedTypeDiscriminators = true,
        //                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        //                DerivedTypes =
        //                {
        //                    new JsonDerivedType(typeof(RuntimeExecutionContext), nameof(RuntimeExecutionContext))
        //                }
        //            };
        //        }

        //        return jsonTypeInfo;
        //    }
        //}

        [JsonDerivedType(typeof(ThreeDimensionalPoint), nameof(ThreeDimensionalPoint))]
        [JsonDerivedType(typeof(TwoDimentionalPoint), nameof(TwoDimentionalPoint))]
        public interface IPoint
        {
            public double X { get; }
            public double Y { get; }
        }

        public interface IThreeDimensionalPoint : IPoint
        {
            public double Z { get; }
        }

        public class TwoDimentionalPoint : IPoint
        {
            public double X { get; init; }
            public double Y { get; init; }
        }

        public class ThreeDimensionalPoint : TwoDimentionalPoint, IThreeDimensionalPoint
        {
            public double Z { get; init; }
        }

        [Fact]
        public void CanSerialize_ExecutionContext()
        {
            var json = JsonSerializer.Serialize(new ThreeDimensionalPoint(), typeof(IPoint), new JsonSerializerOptions()
            {
                WriteIndented = true,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
            });

            json.MatchSnapshot();

            var @object = JsonSerializer.Deserialize(json, typeof(IPoint), new JsonSerializerOptions()
            {
                WriteIndented = true,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
            });

            Assert.IsType<ThreeDimensionalPoint>(@object);
        }
    }
}

