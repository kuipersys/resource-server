// <copyright file="RequestSerialization.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests.Sdk
{
    using Kuiper.Platform.Framework;
    using Kuiper.Platform.Framework.Messages;
    using Kuiper.Platform.ManagementObjects;
    using Kuiper.Platform.Serialization.Serialization;

    using Shouldly;

    using Snapshooter.Xunit;

    public class RequestSerialization
    {
        [Fact]
        public void RequestSerialization_Ok()
        {
            var request = new PutRequest();

            request.Target = new SystemObject
            {
                ApiVersion = "v1",
                Metadata =
                {
                    Name = "test",
                    Namespace = "default",
                },
            };

            var json = request.ObjectToJson(typeof(PlatformRequest), true);

            json.MatchSnapshot();

            var final = json.ObjectFromJson<PlatformRequest>();

            final.ShouldBeOfType<PutRequest>();
        }

        [Fact]
        public void RequestDeserialization_Ok()
        {
            var request = new PutRequest();

            request.Target = new SystemObject
            {
                ApiVersion = "v1",
            };

            var json = request.ObjectToJson(true);

            json.MatchSnapshot();

            var final = json.ObjectFromJson<PlatformRequest>();

            final.ShouldBeOfType<PlatformRequest>();
        }
    }
}
