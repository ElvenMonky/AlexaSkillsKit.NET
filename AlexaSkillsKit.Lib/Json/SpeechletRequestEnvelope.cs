﻿//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.Authentication;

namespace AlexaSkillsKit.Json
{
    public class SpeechletRequestEnvelope
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static SpeechletRequestEnvelope FromJson(SpeechletRequestParser parser, string content) {
            if (String.IsNullOrEmpty(content)) {
                throw new SpeechletValidationException(SpeechletRequestValidationResult.NoContent, "Request content is empty");
            }

            JObject json = JsonConvert.DeserializeObject<JObject>(content, Sdk.DeserializationSettings);
            return FromJson(parser, json);
        }

        public static SpeechletRequestEnvelope FromJson(SpeechletRequestParser parser, JObject json) {
            var version = json.Value<string>("version");
            if (version != null && version != Sdk.VERSION) {
                throw new SpeechletValidationException(SpeechletRequestValidationResult.InvalidVersion, "Request must conform to 1.0 schema.");
            }

            return new SpeechletRequestEnvelope {
                Version = version,
                Request = parser.Parse(json.Value<JObject>("request")),
                Session = Session.FromJson(json.Value<JObject>("session")),
                Context = Context.FromJson(json.Value<JObject>("context"))
            };
        }


        public virtual SpeechletRequest Request {
            get;
            set;
        }

        public virtual Session Session {
            get;
            set;
        }

        public virtual string Version {
            get;
            set;
        }

        public virtual Context Context {
            get;
            set;
        }
    }
}