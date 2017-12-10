//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    [Obsolete("SpeechletAsync base class is obselete and will be removed in a future versions of this library. Implement ISpeechletAsync interface directly instead.")]
    public abstract class SpeechletAsync : ISpeechletAsync
    {
        /// <summary>
        /// Processes Alexa request AND validates request signature
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public async virtual Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage httpRequest) {
            return await (this as ISpeechletAsync).GetResponseAsync(httpRequest);
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public async virtual Task<string> ProcessRequestAsync(string requestContent) {
            return await (this as ISpeechletAsync).ProcessRequestAsync(requestContent);
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature
        /// </summary>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public async virtual Task<string> ProcessRequestAsync(JObject requestJson) {
            return await (this as ISpeechletAsync).ProcessRequestAsync(requestJson);
        }


        /// <summary>
        /// Opportunity to set policy for handling requests with invalid signatures and/or timestamps
        /// </summary>
        /// <returns>true if request processing should continue, otherwise false</returns>
        public virtual Task<bool> OnRequestValidationAsync(
            SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope) {
            
            return Task.FromResult(result == SpeechletRequestValidationResult.OK);
        }


        public abstract Task OnSystemExceptionEncounteredAsync(SystemExceptionEncounteredRequest systemRequest, Context context);

        public abstract Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session, Context context);
        public abstract Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session);
        public abstract Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session);
        public abstract Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session);
    }
}