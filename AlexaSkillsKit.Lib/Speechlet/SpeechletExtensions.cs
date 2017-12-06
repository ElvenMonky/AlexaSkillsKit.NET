//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using AlexaSkillsKit.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public static class SpeechletExtensions
    {
        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public static string ProcessRequest(this ISpeechlet speechlet, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return speechlet.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static string ProcessRequest(this ISpeechletAsync speechletAsync, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return speechletAsync.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechlet speechlet, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return (await speechlet.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechletAsync speechletAsync, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return (await speechletAsync.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        public static string ProcessRequest(this ISpeechlet speechlet, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return speechlet.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static string ProcessRequest(this ISpeechletAsync speechletAsync, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return speechletAsync.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechlet speechlet, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return (await speechlet.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechletAsync speechletAsync, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return (await speechletAsync.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        public static SpeechletResponseEnvelope ProcessRequest(this ISpeechlet speechlet, SpeechletRequestEnvelope requestEnvelope) {
            return AsyncHelpers.RunSync(() => speechlet.ProcessRequestAsync(requestEnvelope));
        }

        public static SpeechletResponseEnvelope ProcessRequest(this ISpeechletAsync speechletAsync, SpeechletRequestEnvelope requestEnvelope) {
            return AsyncHelpers.RunSync(() => speechletAsync.ProcessRequestAsync(requestEnvelope));
        }

        public static async Task<SpeechletResponseEnvelope> ProcessRequestAsync(this ISpeechlet speechlet, SpeechletRequestEnvelope requestEnvelope) {
            return await new SpeechletRequestHandler(speechlet).DoProcessRequestAsync(requestEnvelope);
        }

        public static async Task<SpeechletResponseEnvelope> ProcessRequestAsync(this ISpeechletAsync speechletAsync, SpeechletRequestEnvelope requestEnvelope) {
            return await new SpeechletRequestHandler(speechletAsync).DoProcessRequestAsync(requestEnvelope);
        }

        /// <summary>
        /// Processes Alexa request AND validates request signature
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetResponse(this ISpeechlet speechlet, HttpRequestMessage httpRequest) {
            return AsyncHelpers.RunSync(() => speechlet.GetResponseAsync(httpRequest));
        }

        public static HttpResponseMessage GetResponse(this ISpeechletAsync speechletAsync, HttpRequestMessage httpRequest) {
            return AsyncHelpers.RunSync(() => speechletAsync.GetResponseAsync(httpRequest));
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(this ISpeechlet speechlet, HttpRequestMessage httpRequest) {
            return await new SpeechletRequestHandler(speechlet).GetResponseAsync(httpRequest);
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(this ISpeechletAsync speechletAsync, HttpRequestMessage httpRequest) {
            return await new SpeechletRequestHandler(speechletAsync).GetResponseAsync(httpRequest);
        }
    }
}