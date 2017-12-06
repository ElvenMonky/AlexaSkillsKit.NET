//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AlexaSkillsKit.Json;
using AlexaSkillsKit.Authentication;
using Newtonsoft.Json;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletRequestHandler
    {
        private readonly ISpeechletAsync speechletAsync;

        public SpeechletRequestHandler(ISpeechlet speechlet) {
            speechletAsync = new SpeechletAsyncWrapper(speechlet);
        }

        public SpeechletRequestHandler(ISpeechletAsync speechletAsync) {
            this.speechletAsync = speechletAsync;
        }


        /// <summary>
        /// Processes Alexa request AND validates request signature
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public async virtual Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage httpRequest) {
            SpeechletRequestValidationResult validationResult = SpeechletRequestValidationResult.OK;

            string chainUrl = null;
            if (!httpRequest.Headers.Contains(Sdk.SIGNATURE_CERT_URL_REQUEST_HEADER) ||
                String.IsNullOrEmpty(chainUrl = httpRequest.Headers.GetValues(Sdk.SIGNATURE_CERT_URL_REQUEST_HEADER).First())) {
                validationResult = validationResult | SpeechletRequestValidationResult.NoCertHeader;
            }

            string signature = null;
            if (!httpRequest.Headers.Contains(Sdk.SIGNATURE_REQUEST_HEADER) ||
                String.IsNullOrEmpty(signature = httpRequest.Headers.GetValues(Sdk.SIGNATURE_REQUEST_HEADER).First())) {
                validationResult = validationResult | SpeechletRequestValidationResult.NoSignatureHeader;
            }

            var alexaBytes = await httpRequest.Content.ReadAsByteArrayAsync();
            var alexaContent = Encoding.UTF8.GetString(alexaBytes);
            Debug.WriteLine(alexaContent);

            // attempt to verify signature only if we were able to locate certificate and signature headers
            if (validationResult == SpeechletRequestValidationResult.OK) {
                if (!(await SpeechletRequestSignatureVerifier.VerifyRequestSignatureAsync(alexaBytes, signature, chainUrl))) {
                    validationResult = validationResult | SpeechletRequestValidationResult.InvalidSignature;
                }
            }

            SpeechletRequestEnvelope alexaRequest = null;
            try {
                alexaRequest = SpeechletRequestEnvelope.FromJson(alexaContent);
            }
            catch (Exception ex)
            when (ex is JsonReaderException || ex is InvalidCastException || ex is FormatException) {
                validationResult = validationResult | SpeechletRequestValidationResult.InvalidJson;
            }

            DateTime now = DateTime.UtcNow; // reference time for this request

            // attempt to verify timestamp only if we were able to parse request body
            if (alexaRequest != null) {
                if (!SpeechletRequestTimestampVerifier.VerifyRequestTimestamp(alexaRequest, now)) {
                    validationResult = validationResult | SpeechletRequestValidationResult.InvalidTimestamp;
                }
            }

            if (alexaRequest == null || !await speechletAsync.OnRequestValidationAsync(validationResult, now, alexaRequest)) {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) {
                    ReasonPhrase = validationResult.ToString()
                };
            }

            var alexaResponse = await DoProcessRequestAsync(alexaRequest);
            var json = alexaResponse?.ToJson();
            Debug.WriteLine(json);

            return (alexaResponse == null) ?
                new HttpResponseMessage(HttpStatusCode.InternalServerError) :
                new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        public async Task<SpeechletResponseEnvelope> DoProcessRequestAsync(SpeechletRequestEnvelope requestEnvelope) {
            Session session = requestEnvelope.Session;
            var context = requestEnvelope.Context;
            var request = requestEnvelope.Request;
            ISpeechletResponse response = null;

            // Do session management prior to calling OnSessionStarted and OnIntentAsync 
            // to allow dev to change session values if behavior is not desired
            DoSessionManagement(request as IntentRequest, session);

            if (requestEnvelope.Session.IsNew) {
                await speechletAsync.OnSessionStartedAsync(
                    new SessionStartedRequest(request.RequestId, request.Timestamp, request.Locale), session);
            }

            // process launch request
            if (requestEnvelope.Request is LaunchRequest) {
                response = await speechletAsync.OnLaunchAsync(request as LaunchRequest, session);
            }

            // process audio player request
            else if (requestEnvelope.Request is AudioPlayerRequest) {
                response = await speechletAsync.OnAudioPlayerAsync(request as AudioPlayerRequest, context);
            }

            // process playback controller request
            else if (requestEnvelope.Request is PlaybackControllerRequest) {
                response = await speechletAsync.OnPlaybackControllerAsync(request as PlaybackControllerRequest, context);
            }

            // process system request
            else if (requestEnvelope.Request is SystemExceptionEncounteredRequest) {
                await speechletAsync.OnSystemExceptionEncounteredAsync(request as SystemExceptionEncounteredRequest, context);
            }

            // process intent request
            else if (requestEnvelope.Request is IntentRequest) {
                response = await speechletAsync.OnIntentAsync(request as IntentRequest, session, context);
            }

            // process session ended request
            else if (requestEnvelope.Request is SessionEndedRequest) {
                await speechletAsync.OnSessionEndedAsync(request as SessionEndedRequest, session);
            }

            var responseEnvelope = new SpeechletResponseEnvelope {
                Version = requestEnvelope.Version,
                Response = response,
                SessionAttributes = session.Attributes
            };

            return responseEnvelope;
        }


        /// <summary>
        /// 
        /// </summary>
        private void DoSessionManagement(IntentRequest request, Session session) {
            if (request == null) return;

            if (session.IsNew) {
                session.Attributes[Session.INTENT_SEQUENCE] = request.Intent.Name;
            }
            else {
                // if the session was started as a result of a launch request 
                // a first intent isn't yet set, so set it to the current intent
                if (!session.Attributes.ContainsKey(Session.INTENT_SEQUENCE)) {
                    session.Attributes[Session.INTENT_SEQUENCE] = request.Intent.Name;
                }
                else {
                    session.Attributes[Session.INTENT_SEQUENCE] += Session.SEPARATOR + request.Intent.Name;
                }
            }

            // Auto-session management: copy all slot values from current intent into session
            foreach (var slot in request.Intent.Slots.Values) {
                if (!String.IsNullOrEmpty(slot.Value)) session.Attributes[slot.Name] = slot.Value;
            }
        }
    }
}