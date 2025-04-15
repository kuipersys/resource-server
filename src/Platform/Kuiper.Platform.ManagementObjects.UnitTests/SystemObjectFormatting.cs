// <copyright file="SystemObjectFormatting.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.UnitTests
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.ManagementObjects.v1alpha1;
    using Kuiper.Platform.Serialization.Serialization;

    using Snapshooter.Xunit;

    public class SystemObjectFormatting
    {
        [Fact]
        public void Validate_SystemObject_JsonFormat()
        {
            SystemObject systemObject = new SystemObject()
            {
                ApiVersion = "v1",
                Kind = "ServiceConfig",
                Metadata = new SystemObjectMetadata()
                {
                    Name = "service-a",
                    Namespace = "default",
                    Labels = new Dictionary<string, string>()
                    {
                        { "geo", "us" },
                        { "region", "westus" },
                        { "env", "prod" },
                        { "Xavier", "saves" },
                        { "the", "day" },
                    }
                }
            };

            systemObject
                .ObjectToJson(true)
                .MatchSnapshot();
        }

        [Fact]
        public void Validate_SystemObject_YamlFormat()
        {
            SystemObject systemObject = new SystemObject()
            {
                ApiVersion = "v1",
                Kind = "ServiceConfig",
                Metadata = new SystemObjectMetadata()
                {
                    Name = "service-a",
                    Namespace = "default",
                    Labels = new Dictionary<string, string>()
                    {
                        { "Xavier", "saves" },
                        { "the", "day" },
                        { "geo", "us" },
                        { "region", "westus" },
                        { "env", "prod" }
                    }
                }
            };

            systemObject
                .ObjectToYaml()
                .MatchSnapshot();
        }

        [Fact]
        public void Validate_ResourceDefinition_FromYaml()
        {
            var yaml = File.ReadAllText("./samples/crontab.yml");
            var yamlExample = File.ReadAllText("./samples/crontab.example.yml");
            var resourceDefinition = yaml.ObjectFromYaml<ResourceDefinition>();
            var schema = resourceDefinition.Spec.Versions[0].GetJsonSchema();

            var jsonExample = yamlExample
                .ObjectFromYaml<SystemObject>()
                .ObjectToJson();

            var result = schema.Validate(jsonExample, NJsonSchema.SchemaType.OpenApi3);

            Assert.True(result.Count == 0);

            resourceDefinition
                .ObjectToJson(true)
                .MatchSnapshot();
        }

        //[Fact]
        //public void GenerateSchemaWithNSwag()
        //{
        //    var schema = JsonSchemaGenerator.FromType(typeof(SystemObject), new NewtonsoftJsonSchemaGeneratorSettings()
        //    {
        //        SchemaType = NJsonSchema.SchemaType.OpenApi3,
        //        FlattenInheritanceHierarchy = true,
        //        UseXmlDocumentation = true
        //    });

        //    var objectSchema = schema.Definitions
        //        .First()
        //        .Value;

        //    objectSchema.ToJson()
        //        .JsonToYaml()
        //        .MatchSnapshot();
        //}
    }
}
