namespace AlexaSkillsKit.Speechlet
{
    public interface IDisplaySpeechlet
    {
        SpeechletResponse OnDisplay(DisplayRequest displayRequest, Context context);
    }
}