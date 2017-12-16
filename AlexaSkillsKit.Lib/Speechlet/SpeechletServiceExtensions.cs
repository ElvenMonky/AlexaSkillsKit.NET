using AlexaSkillsKit.Json;
using AlexaSkillsKit.Requests;

namespace AlexaSkillsKit.Speechlet
{
    public static class SpeechletServiceExtensions
    {
        public static void AddStandard(this SpeechletService service, ISpeechletAsync speechlet) {
            service.AddHandler<LaunchRequest>(async (request, session, context) =>
                    await speechlet.OnLaunchAsync(request, session));

            service.AddHandler<IntentRequest>(async (request, session, context) =>
                    await speechlet.OnIntentAsync(request, session, context));

            service.AddHandler<SessionEndedRequest>(async (request, session, context) => {
                await speechlet.OnSessionEndedAsync(request, session);
                return null;
            });

            SpeechletRequestResolver.AddStandard();
        }

        public static void AddStandard(this SpeechletService service, ISpeechlet speechlet) {
            service.AddStandard(new SpeechletAsyncWrapper(speechlet));
        }

        public static void AddSystem(this SpeechletService service, ISystemSpeechletAsync speechlet) {
            service.AddHandler<SystemExceptionEncounteredRequest>(async (request, session, context) => {
                await speechlet.OnSystemExceptionEncounteredAsync(request, context);
                return null;
            });

            SpeechletRequestResolver.AddSystem();
        }

        public static void AddSystem(this SpeechletService service, ISystemSpeechlet speechlet) {
            service.AddSystem(new SystemSpeechletAsyncWrapper(speechlet));
        }
    }
}