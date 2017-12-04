using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.AudioPlayer.Directives
{
    public static class SpeechletResponseBuilderExtentions
    {
        public static ISpeechletResponseBuilder WithAudioPlayerClearQueueDirective(this ISpeechletResponseBuilder builder,
            AudioPlayerClearQueueDirective.ClearBehaviorEnum clearBehavior) {
            return builder.WithDirective(new AudioPlayerClearQueueDirective {
                ClearBehavior = clearBehavior
            });
        }

        public static ISpeechletResponseBuilder WithAudioPlayerPlayDirective(this ISpeechletResponseBuilder builder,
            AudioPlayerPlayDirective.PlayBehaviorEnum playBehavior, AudioItem audioItem) {
            return builder.WithDirective(new AudioPlayerPlayDirective {
                PlayBehavior = playBehavior,
                AudioItem = audioItem
            });
        }

        public static ISpeechletResponseBuilder WithAudioPlayerStopDirective(this ISpeechletResponseBuilder builder) {
            return builder.WithDirective(new AudioPlayerStopDirective());
        }
    }
}