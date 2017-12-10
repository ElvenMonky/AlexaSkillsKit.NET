namespace AlexaSkillsKit.Speechlet
{
    public interface IAudioPlayerSpeechlet
    {
        AudioPlayerResponse OnAudioPlayer(AudioPlayerRequest audioRequest, Context context);
        AudioPlayerResponse OnPlaybackController(PlaybackControllerRequest playbackRequest, Context context);
    }
}