using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.AudioPlayer.Directives
{
    public static class AudioPlayerResponseBuilderExtentions
    {
        public static IAudioPlayerResponseBuilder WithAudioPlayerClearQueueDirective(this IAudioPlayerResponseBuilder builder,
            AudioPlayerClearQueueDirective.ClearBehaviorEnum clearBehavior) {
            return builder.WithDirective(new AudioPlayerClearQueueDirective {
                ClearBehavior = clearBehavior
            });
        }

        public static IAudioPlayerResponseBuilder WithAudioPlayerPlayDirective(this IAudioPlayerResponseBuilder builder,
            AudioPlayerPlayDirective.PlayBehaviorEnum playBehavior, AudioItem audioItem) {
            return builder.WithDirective(new AudioPlayerPlayDirective {
                PlayBehavior = playBehavior,
                AudioItem = audioItem
            });
        }

        public static IAudioPlayerResponseBuilder WithAudioPlayerStopDirective(this IAudioPlayerResponseBuilder builder) {
            return builder.WithDirective(new AudioPlayerStopDirective());
        }
    }
}