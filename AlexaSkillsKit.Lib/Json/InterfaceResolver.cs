using AlexaSkillsKit.Requests;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AlexaSkillsKit.Json
{
    public class InterfaceResolver
    {
        private IDictionary<string, Func<JObject, string, SpeechletRequest>> deserializers
            = new Dictionary<string, Func<JObject, string, SpeechletRequest>>();

        public InterfaceResolver WithDeserializer(string type, Func<JObject, string, SpeechletRequest> fromJson) {
            deserializers.Add(type, fromJson);
            return this;
        }

        public InterfaceResolver WithDefaultDeserializer(Func<JObject, string, SpeechletRequest> fromJson) {
            deserializers.Add(string.Empty, fromJson);
            return this;
        }

        public SpeechletRequest FromJson(string type, JObject json) {
            if (json == null) return null;
            if (deserializers.ContainsKey(type)) return deserializers[type](json, type);
            if (deserializers.ContainsKey(string.Empty)) return deserializers[string.Empty](json, type);
            return null;
        }
    }
}