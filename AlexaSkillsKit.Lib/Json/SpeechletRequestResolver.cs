using AlexaSkillsKit.Speechlet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlexaSkillsKit.Json
{
    public class SpeechletRequestResolver
    {
        private IDictionary<string, InterfaceResolver> resolvers
            = new Dictionary<string, InterfaceResolver>();

        private SpeechletRequest FromJson(string type, string subtype, JObject json) {
            if (json == null || !resolvers.ContainsKey(type)) return null;

            return resolvers[type].FromJson(subtype, json);
        }

        public SpeechletRequest FromJson(JObject json) {
            var requestTypeParts = json?.Value<string>("type")?.Split('.');
            if (requestTypeParts == null) {
                throw new ArgumentException("json");
            }

            var requestType = requestTypeParts.Length > 1 ? requestTypeParts[0] : string.Empty;
            var requestSubtype = requestTypeParts.Last();
            var request = FromJson(requestType, requestSubtype, json);
            if (request == null) {
                throw new ArgumentException("json");
            }

            return request;
        }

        public void AddInterface(string name, InterfaceResolver resolver) {
            resolvers[name] = resolver;
        }

        public void AddStandard() {
            var standardResolver = new InterfaceResolver()
                .WithDeserializer(nameof(LaunchRequest), (json, subtype) => new LaunchRequest(json))
                .WithDeserializer(nameof(IntentRequest), (json, subtype) => new IntentRequest(json))
                .WithDeserializer(nameof(SessionEndedRequest), (json, subtype) => new SessionEndedRequest(json));
            AddInterface(string.Empty, standardResolver);
        }

        public void AddSystem() {
            var systemResolver = new InterfaceResolver()
                .WithDeserializer("ExceptionEncountered", (json, subtype) => new SystemExceptionEncounteredRequest(json, subtype));
            AddInterface("System", systemResolver);
        }
    }
}