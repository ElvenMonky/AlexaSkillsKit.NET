using System.Collections.Generic;

namespace AlexaSkillsKit.Speechlet
{
    public class AudioPlayerResponseBuilder : IAudioPlayerResponseBuilder
    {
        private AudioPlayerResponse response = new AudioPlayerResponse();
        private IList<AudioPlayerDirective> directives = new List<AudioPlayerDirective>();

        public AudioPlayerResponse Build() {
            if (directives.Count > 0) {
                response.Directives = directives;
            }
            return response;
        }

        public IAudioPlayerResponseBuilder WithDirective(AudioPlayerDirective directive) {
            directives.Add(directive);
            return this;
        }
    }
}