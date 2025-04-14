// <copyright file="PropertyBag.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(PropertyBagConverter))]
    public class PropertyBag : Dictionary<string, object>, IDictionary<string, object>
    {
        public T Get<T>(string key)
        {
            if (this.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public void Set<T>(string key, T value)
        {
            this[key] = value;
        }
    }
}
