//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using AlexaSkillsKit.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AlexaSkillsKit.Speechlet
{
    public abstract class SpeechletRequest
    {
        public static SpeechletRequest FromJson(JObject json) {
            var requestTypeParts = json?.Value<string>("type")?.Split('.');
            if (requestTypeParts == null) {
                throw new ArgumentException("json");
            }

            var requestType = requestTypeParts.Length > 1 ? requestTypeParts[0] : string.Empty;
            var requestSubtype = requestTypeParts.Last();
            var request = SpeechletRequestResolver.FromJson(requestType, requestSubtype, json);
            if (request == null) {
                throw new ArgumentException("json");
            }

            return request;
        }

        protected SpeechletRequest(JObject json) {
            RequestId = json.Value<string>("requestId");
            Timestamp = DateTimeHelpers.FromAlexaTimestamp(json);
            Locale = json.Value<string>("locale");
        }

        protected SpeechletRequest(SpeechletRequest other) {
            RequestId = other.RequestId;
            Timestamp = other.Timestamp;
            Locale = other.Locale;
        }

        public string RequestId {
            get;
            private set;
        }

        public DateTime Timestamp {
            get;
            private set;
        }

        public string Locale {
            get;
            private set;
        }
    }
}