using AlexaSkillsKit.Requests;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public class SystemSpeechletAsyncWrapper: ISystemSpeechletAsync
    {
        private readonly ISystemSpeechlet speechlet;

        public SystemSpeechletAsyncWrapper(ISystemSpeechlet speechlet) {
            this.speechlet = speechlet;
        }

        public async Task OnSystemExceptionEncounteredAsync(SystemExceptionEncounteredRequest systemRequest, Context context) {
            speechlet.OnSystemExceptionEncountered(systemRequest, context);
        }
    }
}