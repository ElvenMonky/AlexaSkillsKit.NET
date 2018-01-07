using AlexaSkillsKit.Interfaces.AudioPlayer;
using AlexaSkillsKit.Interfaces.Display;
using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Json
{
    public static class SpeechletRequestParserExtensions
    {
        public static void AddStandard(this SpeechletRequestParser parser) {
            parser.AddInterface(string.Empty, (subtype, json) => {
                switch (subtype) {
                    case "LaunchRequest":
                        return new LaunchRequest(json);
                    case "IntentRequest":
                        return new IntentRequest(json);
                    case "SessionEndedRequest":
                        return new SessionEndedRequest(json);
                }
                return null;
            });
        }

        public static void AddSystem(this SpeechletRequestParser parser) {
            parser.AddInterface(SystemRequest.TypeName, (subtype, json) => {
                switch (subtype) {
                    case "ExceptionEncountered":
                        return new SystemExceptionEncounteredRequest(subtype, json);
                }
                return null;
            });
        }
    }
}