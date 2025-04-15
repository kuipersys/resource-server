// <copyright file="ByteExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Serialization.Serialization
{
    using System;
    using System.Linq;
    using System.Text;

    public static class ByteExtensions
    {
        public static T YamlBytesToObject<T>(this byte[] bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var content = Encoding.UTF8.GetString(bytes);

            return content.ObjectFromYaml<T>();
        }

        public static T JsonBytesToObject<T>(this byte[] bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var content = Encoding.UTF8.GetString(bytes);

            return content.ObjectFromJson<T>();
        }

        public static byte[] ObjectToYamlBytes<T>(this T @object)
            => Encoding.UTF8.GetBytes(@object.ObjectToYaml());

        public static byte[] ObjectToJsonBytes<T>(this T @object)
            => Encoding.UTF8.GetBytes(@object.ObjectToJson());
    }
}
