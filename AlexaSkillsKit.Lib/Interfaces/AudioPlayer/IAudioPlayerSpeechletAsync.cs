using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public interface IAudioPlayerSpeechletAsync
    {
        Task<AudioPlayerResponse> OnAudioPlayerAsync(AudioPlayerRequest audioRequest, Context context);
        Task<AudioPlayerResponse> OnPlaybackControllerAsync(PlaybackControllerRequest playbackRequest, Context context);
    }
}