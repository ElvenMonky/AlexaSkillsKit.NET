using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AlexaSkillsKit.Json
{
    public class Deserializer<T>
    {
        private static IDictionary<string, Func<JObject, T>> deserializers = new Dictionary<string, Func<JObject, T>>();

        public static void RegisterDeserializer(string name, Func<JObject, T> fromJson) {
            deserializers.Add(name, fromJson);
        }

        public static T FromJson(JProperty json) {
            if (json == null || !deserializers.ContainsKey(json.Name)) return default(T);

            return deserializers[json.Name](json.Value as JObject);
        }

        public static T FromJson(string name, JObject json) {
            if (json == null || !deserializers.ContainsKey(name)) return default(T);

            return deserializers[name](json);
        }
    }
}