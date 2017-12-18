using AlexaSkillsKit.Json;
using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.AudioPlayer
{
    public static class AudioPlayerSpeechletServiceExtensions
    {
        public static void AddAudioPlayer(this SpeechletService service, IAudioPlayerSpeechlet speechlet) {
            service.AddAudioPlayer(new AudioPlayerSpeechletAsyncWrapper(speechlet));
        }

        public static void AddAudioPlayer(this SpeechletService service, IAudioPlayerSpeechletAsync speechlet) {
            Deserializer<ISpeechletInterface>.RegisterDeserializer("AudioPlayer", AudioPlayerInterface.FromJson);

            var audioPlayerResolver = new InterfaceResolver()
                .WithDefaultDeserializer((json, subtype) => new AudioPlayerRequest(json, subtype))
                .WithDeserializer("PlaybackFailed", (json, subtype) => new AudioPlayerPlaybackFailedRequest(json, subtype));
            service.RequestResolver.AddInterface("AudioPlayer", audioPlayerResolver);

            service.AddHandler<AudioPlayerRequest>(
                async (request, context) => await speechlet.OnAudioPlayerAsync(request, context));

            var playbackControllerResolver = new InterfaceResolver()
                .WithDefaultDeserializer((json, subtype) => new PlaybackControllerRequest(json, subtype));
            service.RequestResolver.AddInterface("PlaybackController", playbackControllerResolver);

            service.AddHandler<PlaybackControllerRequest>(
                async (request, context) => await speechlet.OnPlaybackControllerAsync(request, context));
        }
    }
}