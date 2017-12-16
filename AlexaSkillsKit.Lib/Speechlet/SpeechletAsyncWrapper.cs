using AlexaSkillsKit.Requests;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public class SpeechletAsyncWrapper: ISpeechletAsync
    {
        private readonly ISpeechlet speechlet;

        public SpeechletAsyncWrapper(ISpeechlet speechlet) {
            this.speechlet = speechlet;
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
    }
}