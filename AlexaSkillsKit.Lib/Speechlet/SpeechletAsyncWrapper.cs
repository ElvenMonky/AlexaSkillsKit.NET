using System;
using System.Threading.Tasks;
using AlexaSkillsKit.Json;
using AlexaSkillsKit.Authentication;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletAsyncWrapper: ISpeechletAsync
    {
        private readonly ISpeechlet speechlet;

        public SpeechletAsyncWrapper(ISpeechlet speechlet) {
            this.speechlet = speechlet;
        }

        public async Task OnSystemExceptionEncounteredAsync(SystemExceptionEncounteredRequest systemRequest, Context context) {
            speechlet.OnSystemExceptionEncountered(systemRequest, context);
        }

        public async Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session, Context context) {
            return speechlet.OnIntent(intentRequest, session, context);
        }

        public async Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session) {
            return speechlet.OnLaunch(launchRequest, session);
        }


        public async Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session) {
            speechlet.OnSessionEnded(sessionEndedRequest, session);
        }

        public async Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session) {
            speechlet.OnSessionStarted(sessionStartedRequest, session);
        }

        public async Task<bool> OnRequestValidationAsync(SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope) {
            return speechlet.OnRequestValidation(result, referenceTimeUtc, requestEnvelope);
        }
    }
}