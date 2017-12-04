namespace AlexaSkillsKit.Speechlet
{
    public interface IAudioPlayerResponseBuilder
    {
        AudioPlayerResponse Build();
        IAudioPlayerResponseBuilder WithDirective(AudioPlayerDirective directive);
    }
}