using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public class AudioPlayerSpeechletAsyncWrapper : IAudioPlayerSpeechletAsync
    {
        private readonly IAudioPlayerSpeechlet speechlet;

        public AudioPlayerSpeechletAsyncWrapper(IAudioPlayerSpeechlet speechlet) {
            this.speechlet = speechlet;
        }

        public async Task<AudioPlayerResponse> OnAudioPlayerAsync(AudioPlayerRequest audioRequest, Context context) {
            return speechlet.OnAudioPlayer(audioRequest, context);
        }

        public async Task<AudioPlayerResponse> OnPlaybackControllerAsync(PlaybackControllerRequest playbackRequest, Context context) {
            return speechlet.OnPlaybackController(playbackRequest, context);
        }
    }
}