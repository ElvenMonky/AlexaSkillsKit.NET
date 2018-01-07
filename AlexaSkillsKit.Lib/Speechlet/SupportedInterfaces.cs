﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using AlexaSkillsKit.Json;

namespace AlexaSkillsKit.Speechlet
{
    /// <summary>
    /// https://developer.amazon.com/docs/custom-skills/request-and-response-json-reference.html#system-object
    /// </summary>
    public class SupportedInterfaces : Dictionary<string, ISpeechletInterface>
    {
        public static SupportedInterfaces FromJson(JObject json) {
            if (json == null) return null;

            var dictionary = json.Children<JProperty>()
                .ToDictionary(x => x.Name, x => Deserializer<ISpeechletInterface>.FromJson(x));

            return new SupportedInterfaces(dictionary);
        }

        private SupportedInterfaces(IDictionary<string, ISpeechletInterface> dictionary) : base(dictionary) { }
    }
}