using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.VideoApp.Directives
{
    public static class SpeechletResponseBuilderExtentions
    {
        public static ISpeechletResponseBuilder WithVideoAppLaunchDirective(this ISpeechletResponseBuilder builder,
            VideoItem videoItem) {
            return builder.KeepSession().WithDirective(new VideoAppLaunchDirective {
                VideoItem = videoItem
            });
        }
    }
}