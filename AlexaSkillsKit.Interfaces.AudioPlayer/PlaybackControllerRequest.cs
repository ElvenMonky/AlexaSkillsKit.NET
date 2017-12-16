using AlexaSkillsKit.Requests;
using Newtonsoft.Json.Linq;

namespace AlexaSkillsKit.Interfaces.AudioPlayer
{
    /// <summary>
    /// https://developer.amazon.com/docs/custom-skills/playback-controller-interface-reference.html#requests
    /// </summary>
    public class PlaybackControllerRequest : ExtendedSpeechletRequest
    {
        public PlaybackControllerRequest(JObject json, string subtype) : base(json, subtype) {
        }
    }
}