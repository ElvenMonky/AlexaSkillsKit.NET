using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Json
{
    public static class SpeechletRequestResolverExtensions
    {
        public static void AddStandard(this SpeechletRequestResolver resolver) {
            var standardResolver = new InterfaceResolver()
                .WithDeserializer(nameof(LaunchRequest), (json, subtype) => new LaunchRequest(json))
                .WithDeserializer(nameof(IntentRequest), (json, subtype) => new IntentRequest(json))
                .WithDeserializer(nameof(SessionEndedRequest), (json, subtype) => new SessionEndedRequest(json));
            resolver.AddInterface(string.Empty, standardResolver);
        }

        public static void AddSystem(this SpeechletRequestResolver resolver) {
            var systemResolver = new InterfaceResolver()
                .WithDeserializer("ExceptionEncountered", (json, subtype) => new SystemExceptionEncounteredRequest(json, subtype));
            resolver.AddInterface("System", systemResolver);
        }
    }
}