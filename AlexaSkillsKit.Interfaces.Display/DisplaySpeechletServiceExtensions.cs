using AlexaSkillsKit.Json;
using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.Display
{
    public static class DisplaySpeechletServiceExtensions
    {
        public static void AddDisplay(this SpeechletService service, IDisplaySpeechlet speechlet) {
            service.AddDisplay(new DisplaySpeechletAsyncWrapper(speechlet));
        }

        public static void AddDisplay(this SpeechletService service, IDisplaySpeechletAsync speechlet) {
            Deserializer<ISpeechletInterface>.RegisterDeserializer("Display", DisplayInterface.FromJson);

            service.RequestParser.AddInterface("Display", (subtype, json) => new DisplayRequest(subtype, json));

            service.AddHandler<DisplayRequest>(async (request, context) => await speechlet.OnDisplayAsync(request, context));
        }
    }
}