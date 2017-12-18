using AlexaSkillsKit.Speechlet;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AlexaSkillsKit.Json
{
    public static class SpeechletRequestResolver
    {
        private static IDictionary<string, InterfaceResolver> resolvers
            = new Dictionary<string, InterfaceResolver>();

        public static SpeechletRequest FromJson(string type, string subtype, JObject json) {
            if (json == null || !resolvers.ContainsKey(type)) return null;

            return resolvers[type].FromJson(subtype, json);
        }

        public static void AddInterface(string name, InterfaceResolver resolver) {
            resolvers[name] = resolver;
        }

        public static void AddStandard() {
            var standardResolver = new InterfaceResolver()
                .WithDeserializer(nameof(LaunchRequest), (json, subtype) => new LaunchRequest(json))
                .WithDeserializer(nameof(IntentRequest), (json, subtype) => new IntentRequest(json))
                .WithDeserializer(nameof(SessionEndedRequest), (json, subtype) => new SessionEndedRequest(json));
            AddInterface(string.Empty, standardResolver);
        }

        public static void AddSystem() {
            var systemResolver = new InterfaceResolver()
                .WithDeserializer("ExceptionEncountered", (json, subtype) => new SystemExceptionEncounteredRequest(json, subtype));
            AddInterface("System", systemResolver);
        }
    }
}