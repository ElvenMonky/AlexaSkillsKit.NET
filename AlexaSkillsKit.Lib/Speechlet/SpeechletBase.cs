//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletBase
    {
        public SpeechletService Service { get; } = new SpeechletService();

        public SpeechletBase() {
            Service.ValidationHandler = OnRequestValidation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public string ProcessRequest(string requestContent) {
            var request = SpeechletRequestEnvelope.FromJson(requestContent);
            return AsyncHelpers.RunSync(async () => (await Service.ProcessRequestAsync(request))?.ToJson());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public string ProcessRequest(JObject requestJson) {
            var request = SpeechletRequestEnvelope.FromJson(requestJson);
            return AsyncHelpers.RunSync(async () => (await Service.ProcessRequestAsync(request))?.ToJson());
        }


        /// <summary>
        /// Opportunity to set policy for handling requests with invalid signatures and/or timestamps
        /// </summary>
        /// <returns>true if request processing should continue, otherwise false</returns>
        public virtual bool OnRequestValidation(
            SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope) {
            
            return result == SpeechletRequestValidationResult.OK;
        }
    }
}