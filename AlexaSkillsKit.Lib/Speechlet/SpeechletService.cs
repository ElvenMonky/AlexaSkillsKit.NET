using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletService
    {
        private IDictionary<Type, Func<SpeechletRequest, Session, Context, Task<ISpeechletResponse>>> handlers
            = new Dictionary<Type, Func<SpeechletRequest, Session, Context, Task<ISpeechletResponse>>>();

        public Func<SpeechletRequestValidationResult, DateTime, SpeechletRequestEnvelope, bool> ValidationHandler { get; set; }

        public SpeechletRequestResolver RequestResolver { get; } = new SpeechletRequestResolver();

        public void AddHandler<T>(Func<T, Session, Context, Task<ISpeechletResponse>> handler) where T : SpeechletRequest {
            handlers[typeof(T)] = async (request, session, context) => await handler(request as T, session, context);
        }

        public void AddHandler<T>(Func<T, Context, Task<ISpeechletResponse>> handler) where T : SpeechletRequest {
            handlers[typeof(T)] = async (request, session, context) => await handler(request as T, context);
        }

        public async Task<SpeechletRequestEnvelope> GetRequestAsync(string content, string chainUrl, string signature) {
            var validationResult = SpeechletRequestValidationResult.OK;

            if (string.IsNullOrEmpty(chainUrl)) {
                validationResult |= SpeechletRequestValidationResult.NoCertHeader;
            }

            if (string.IsNullOrEmpty(signature)) {
                validationResult |= SpeechletRequestValidationResult.NoSignatureHeader;
            }

            // attempt to verify signature only if we were able to locate certificate and signature headers
            if (validationResult == SpeechletRequestValidationResult.OK) {
                var alexaBytes = Encoding.UTF8.GetBytes(content);

                if (!await SpeechletRequestSignatureVerifier.VerifyRequestSignatureAsync(alexaBytes, signature, chainUrl)) {
                    validationResult |= SpeechletRequestValidationResult.InvalidSignature;
                }
            }

            SpeechletRequestEnvelope result = null;
            try {


                result = SpeechletRequestEnvelope.FromJson(RequestResolver, content);
            }
            catch (SpeechletValidationException ex) {
                validationResult |= ex.ValidationResult;
            }
            catch (Exception ex)
            when (ex is JsonReaderException || ex is InvalidCastException || ex is FormatException) {
                validationResult |= SpeechletRequestValidationResult.InvalidJson;
            }

            var success = false;

            // attempt to verify timestamp only if we were able to parse request body
            if (result != null) {
                var now = DateTime.UtcNow; // reference time for this request

                if (!SpeechletRequestTimestampVerifier.VerifyRequestTimestamp(result, now)) {
                    validationResult |= SpeechletRequestValidationResult.InvalidTimestamp;
                }

                success = ValidationHandler?.Invoke(validationResult, now, result) ?? (validationResult == SpeechletRequestValidationResult.OK);
            }

            if (!success) {
                throw new SpeechletValidationException(validationResult);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        public async Task<SpeechletResponseEnvelope> ProcessRequestAsync(SpeechletRequestEnvelope requestEnvelope) {
            var session = requestEnvelope.Session;
            var context = requestEnvelope.Context;
            var request = requestEnvelope.Request;
            ISpeechletResponse response = null;

            if (session != null) {
                // Do session management prior to calling OnSessionStarted and OnIntentAsync 
                // to allow dev to change session values if behavior is not desired
                DoSessionManagement(request as IntentRequest, session);

                if (session.IsNew && handlers.ContainsKey(typeof(SessionStartedRequest))) {
                    await handlers[typeof(SessionStartedRequest)].Invoke(new SessionStartedRequest(request), session, context);
                }
            }

            foreach (var pair in handlers) {
                if (pair.Key.GetTypeInfo().IsAssignableFrom(request.GetType().GetTypeInfo())) {
                    response = await pair.Value(request, session, context);
                    break;
                }
            }

            var responseEnvelope = new SpeechletResponseEnvelope {
                Version = requestEnvelope.Version,
                Response = response,
                SessionAttributes = session?.Attributes
            };

            return responseEnvelope;
        }


        /// <summary>
        /// 
        /// </summary>
        private void DoSessionManagement(IntentRequest request, Session session) {
            if (request == null) return;

            if (session.Attributes == null) {
                session.Attributes = new Dictionary<string, string>();
            }

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