namespace AlexaSkillsKit.Speechlet
{
    public static class AudioPlayerSpeechletExtensions
    {
        public static void UseAudioPlayer(this IAudioPlayerSpeechlet speechlet) {
            UseAudioPlayer(new AudioPlayerSpeechletAsyncWrapper(speechlet));
        }

        public static void UseAudioPlayer(this IAudioPlayerSpeechletAsync speechlet) {
            var audioPlayerResolver = new InterfaceResolver()
                .WithDefaultDeserializer((json, subtype) => new AudioPlayerRequest(json, subtype))
                .WithDeserializer("PlaybackFailed", (json, subtype) => new AudioPlayerPlaybackFailedRequest(json, subtype));
            SpeechletRequestResolver.UseInterface("AudioPlayer", audioPlayerResolver);

            SpeechletRequestHandler.UseInterface<AudioPlayerRequest>(
                async (request, context) => await speechlet.OnAudioPlayerAsync(request, context));

            var playbackControllerResolver = new InterfaceResolver()
                .WithDefaultDeserializer((json, subtype) => new PlaybackControllerRequest(json, subtype));
            SpeechletRequestResolver.UseInterface("PlaybackController", playbackControllerResolver);

            SpeechletRequestHandler.UseInterface<PlaybackControllerRequest>(
                async (request, context) => await speechlet.OnPlaybackControllerAsync(request, context));
        }
    }
}