﻿//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletBase
    {
        public SpeechletService Service { get; } = new SpeechletService();

        public SpeechletBase() {
            Service.ValidationHandler = OnRequestValidation;
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public string ProcessRequest(string requestContent) {
            return AsyncHelpers.RunSync(async () => await ProcessRequestAsync(requestContent));
        }

        public async Task<string> ProcessRequestAsync(string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(Service.RequestResolver, requestContent);
            return (await Service.ProcessRequestAsync(requestEnvelope))?.ToJson();
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public virtual string ProcessRequest(JObject requestJson) {
            return AsyncHelpers.RunSync(async () => await ProcessRequestAsync(requestJson));
        }

        public async Task<string> ProcessRequestAsync(JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(Service.RequestResolver, requestJson);
            return (await Service.ProcessRequestAsync(requestEnvelope))?.ToJson();
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