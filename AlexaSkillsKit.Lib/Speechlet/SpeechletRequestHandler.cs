﻿//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using AlexaSkillsKit.Json;
using AlexaSkillsKit.Authentication;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletRequestHandler
    {
        private readonly ISpeechlet speechlet;
        private readonly ISpeechletAsync speechletAsync;

        public SpeechletRequestHandler(ISpeechlet speechlet) {
            this.speechlet = speechlet;
        }

        public SpeechletRequestHandler(ISpeechletAsync speechletAsync) {
            this.speechletAsync = speechletAsync;
        }

        public HttpResponseMessage GetResponse(HttpRequestMessage httpRequest) {
            return AsyncHelpers.RunSync(() => GetResponseAsync(httpRequest));
        }

        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public string ProcessRequest(string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return AsyncHelpers.RunSync(() => DoProcessRequestAsync(requestEnvelope));
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature
        /// </summary>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public string ProcessRequest(JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return AsyncHelpers.RunSync(() => DoProcessRequestAsync(requestEnvelope));
        }

        /// <summary>
        /// Processes Alexa request AND validates request signature
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public async virtual Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage httpRequest) {
            SpeechletRequestValidationResult validationResult = SpeechletRequestValidationResult.OK;
            DateTime now = DateTime.UtcNow; // reference time for this request

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
            when (ex is Newtonsoft.Json.JsonReaderException || ex is InvalidCastException || ex is FormatException) {
                validationResult = validationResult | SpeechletRequestValidationResult.InvalidJson;
            }

            // attempt to verify timestamp only if we were able to parse request body
            if (alexaRequest != null) {
                if (!SpeechletRequestTimestampVerifier.VerifyRequestTimestamp(alexaRequest, now)) {
                    validationResult = validationResult | SpeechletRequestValidationResult.InvalidTimestamp;
                }
            }

            if (alexaRequest == null || !await OnRequestValidationAsync(validationResult, now, alexaRequest)) {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) {
                    ReasonPhrase = validationResult.ToString()
                };
            }

            string alexaResponse = await DoProcessRequestAsync(alexaRequest);
            Debug.WriteLine(alexaResponse);

            HttpResponseMessage httpResponse;
            if (alexaResponse == null) {
                httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            else {
                httpResponse = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(alexaResponse, Encoding.UTF8, "application/json")
                };
            }

            return httpResponse;
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature 
        /// </summary>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public async virtual Task<string> ProcessRequestAsync(string requestContent) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestContent);
            return await DoProcessRequestAsync(requestEnvelope);
        }


        /// <summary>
        /// Processes Alexa request but does NOT validate request signature
        /// </summary>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public async virtual Task<string> ProcessRequestAsync(JObject requestJson) {
            var requestEnvelope = SpeechletRequestEnvelope.FromJson(requestJson);
            return await DoProcessRequestAsync(requestEnvelope);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        private async Task<string> DoProcessRequestAsync(SpeechletRequestEnvelope requestEnvelope) {
            Session session = requestEnvelope.Session;
            var context = requestEnvelope.Context;
            var request = requestEnvelope.Request;
            ISpeechletResponse response = null;

            // Do session management prior to calling OnSessionStarted and OnIntentAsync 
            // to allow dev to change session values if behavior is not desired
            DoSessionManagement(request as IntentRequest, session);

            if (requestEnvelope.Session.IsNew) {
                await OnSessionStartedAsync(
                    new SessionStartedRequest(request.RequestId, request.Timestamp, request.Locale), session);
            }

            // process launch request
            if (requestEnvelope.Request is LaunchRequest) {
                response = await OnLaunchAsync(request as LaunchRequest, session);
            }

            // process audio player request
            else if (requestEnvelope.Request is AudioPlayerRequest) {
                response = await OnAudioPlayerAsync(request as AudioPlayerRequest, context);
            }

            // process playback controller request
            else if (requestEnvelope.Request is PlaybackControllerRequest) {
                response = await OnPlaybackControllerAsync(request as PlaybackControllerRequest, context);
            }

            // process system request
            else if (requestEnvelope.Request is SystemExceptionEncounteredRequest) {
                await OnSystemExceptionEncounteredAsync(request as SystemExceptionEncounteredRequest, context);
            }

            // process intent request
            else if (requestEnvelope.Request is IntentRequest) {
                response = await OnIntentAsync(request as IntentRequest, session, context);
            }

            // process session ended request
            else if (requestEnvelope.Request is SessionEndedRequest) {
                await OnSessionEndedAsync(request as SessionEndedRequest, session);
            }

            var responseEnvelope = new SpeechletResponseEnvelope {
                Version = requestEnvelope.Version,
                Response = response,
                SessionAttributes = session.Attributes
            };
            return responseEnvelope.ToJson();
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

        private async Task<AudioPlayerResponse> OnAudioPlayerAsync(AudioPlayerRequest audioRequest, Context context) {
            return speechlet != null ?
                speechlet.OnAudioPlayer(audioRequest, context)
                :
                await speechletAsync.OnAudioPlayerAsync(audioRequest, context);
        }

        private async Task<AudioPlayerResponse> OnPlaybackControllerAsync(PlaybackControllerRequest playbackRequest, Context context) {
            return speechlet is ISpeechlet ?
                (speechlet as ISpeechlet).OnPlaybackController(playbackRequest, context)
                :
                await (speechlet as ISpeechletAsync).OnPlaybackControllerAsync(playbackRequest, context);
        }

        private async Task OnSystemExceptionEncounteredAsync(SystemExceptionEncounteredRequest systemRequest, Context context) {
            if (speechlet is ISpeechlet)
                (speechlet as ISpeechlet).OnSystemExceptionEncountered(systemRequest, context);
            else
                await (speechlet as ISpeechletAsync).OnSystemExceptionEncounteredAsync(systemRequest, context);
        }

        private async Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session, Context context) {
            return speechlet is ISpeechlet ?
                (speechlet as ISpeechlet).OnIntent(intentRequest, session, context)
                :
                await (speechlet as ISpeechletAsync).OnIntentAsync(intentRequest, session, context);
        }

        private async Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session) {
            return speechlet is ISpeechlet ?
                (speechlet as ISpeechlet).OnLaunch(launchRequest, session)
                :
                await (speechlet as ISpeechletAsync).OnLaunchAsync(launchRequest, session);
        }


        private async Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session) {
            if (speechlet is ISpeechlet)
                (speechlet as ISpeechlet).OnSessionEnded(sessionEndedRequest, session);
            else
                await (speechlet as ISpeechletAsync).OnSessionEndedAsync(sessionEndedRequest, session);
        }

        private async Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session) {
            if (speechlet is ISpeechlet)
                (speechlet as ISpeechlet).OnSessionStarted(sessionStartedRequest, session);
            else
                await (speechlet as ISpeechletAsync).OnSessionStartedAsync(sessionStartedRequest, session);
        }

        private async Task<bool> OnRequestValidationAsync(SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope) {
            return speechlet is ISpeechlet ?
                (speechlet as ISpeechlet).OnRequestValidation(result, referenceTimeUtc, requestEnvelope)
                :
                await (speechlet as ISpeechletAsync).OnRequestValidationAsync(result, referenceTimeUtc, requestEnvelope);
        }
    }
}