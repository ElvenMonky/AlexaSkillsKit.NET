using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AlexaSkillsKit.Speechlet
{
    public static class SpeechletRequestResolver
    {
        private static IDictionary<string, InterfaceResolver> resolvers
            = new Dictionary<string, InterfaceResolver>();

        public static SpeechletRequest FromJson(string type, string subtype, JObject json) {
            if (json == null || !resolvers.ContainsKey(type)) return null;

            return resolvers[type].FromJson(subtype, json);
        }

        public static void UseInterface(string name, InterfaceResolver resolver) {
            resolvers[name] = resolver;
        }
    }
}