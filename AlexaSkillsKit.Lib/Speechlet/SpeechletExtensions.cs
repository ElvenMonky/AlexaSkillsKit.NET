using AlexaSkillsKit.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public static class SpeechletExtensions
    {
        public static void UseStandard(this ISpeechlet speechlet) {
            new SpeechletAsyncWrapper(speechlet).UseStandard();
        }

        public static void UseStandard(this ISpeechletAsync speechlet) {
            var standardResolver = new InterfaceResolver()
                .WithDeserializer(nameof(LaunchRequest), (json, subtype) => new LaunchRequest(json))
                .WithDeserializer(nameof(IntentRequest), (json, subtype) => new IntentRequest(json))
                .WithDeserializer(nameof(SessionEndedRequest), (json, subtype) => new SessionEndedRequest(json));
            SpeechletRequestResolver.UseInterface(string.Empty, standardResolver);

            var systemResolver = new InterfaceResolver()
                .WithDeserializer("ExceptionEncountered", (json, subtype) => new SystemExceptionEncounteredRequest(json, subtype));
            SpeechletRequestResolver.UseInterface("System", systemResolver);

            SpeechletRequestHandler.UseStandard(speechlet);
        }

        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public static string ProcessRequest(this ISpeechlet speechlet, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return speechlet.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static string ProcessRequest(this ISpeechletAsync speechlet, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return speechlet.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechlet speechlet, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return (await speechlet.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechletAsync speechlet, string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return (await speechlet.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        public static string ProcessRequest(this ISpeechlet speechlet, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return speechlet.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static string ProcessRequest(this ISpeechletAsync speechlet, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return speechlet.ProcessRequest(requestEnvelope)?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechlet speechlet, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return (await speechlet.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        public static async Task<string> ProcessRequestAsync(this ISpeechletAsync speechlet, JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return (await speechlet.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }

        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        public static SpeechletResponseEnvelope ProcessRequest(this ISpeechlet speechlet, SpeechletRequestEnvelope requestEnvelope) {
            return AsyncHelpers.RunSync(() => speechlet.ProcessRequestAsync(requestEnvelope));
        }

        public static SpeechletResponseEnvelope ProcessRequest(this ISpeechletAsync speechlet, SpeechletRequestEnvelope requestEnvelope) {
            return AsyncHelpers.RunSync(() => speechlet.ProcessRequestAsync(requestEnvelope));
        }

        public static async Task<SpeechletResponseEnvelope> ProcessRequestAsync(this ISpeechlet speechlet, SpeechletRequestEnvelope requestEnvelope) {
            return await new SpeechletRequestHandler(speechlet).DoProcessRequestAsync(requestEnvelope);
        }

        public static async Task<SpeechletResponseEnvelope> ProcessRequestAsync(this ISpeechletAsync speechlet, SpeechletRequestEnvelope requestEnvelope) {
            return await new SpeechletRequestHandler(speechlet).DoProcessRequestAsync(requestEnvelope);
        }

        /// <summary>
        /// Processes Alexa request AND validates request signature
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetResponse(this ISpeechlet speechlet, HttpRequestMessage httpRequest) {
            return AsyncHelpers.RunSync(() => speechlet.GetResponseAsync(httpRequest));
        }

        public static HttpResponseMessage GetResponse(this ISpeechletAsync speechlet, HttpRequestMessage httpRequest) {
            return AsyncHelpers.RunSync(() => speechlet.GetResponseAsync(httpRequest));
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(this ISpeechlet speechlet, HttpRequestMessage httpRequest) {
            return await new SpeechletRequestHandler(speechlet).GetResponseAsync(httpRequest);
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(this ISpeechletAsync speechlet, HttpRequestMessage httpRequest) {
            return await new SpeechletRequestHandler(speechlet).GetResponseAsync(httpRequest);
        }
    }
}