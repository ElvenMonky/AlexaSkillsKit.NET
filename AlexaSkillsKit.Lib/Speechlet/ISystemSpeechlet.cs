namespace AlexaSkillsKit.Speechlet
{
    public interface ISystemSpeechlet
    {
        void OnSystemExceptionEncountered(SystemExceptionEncounteredRequest systemRequest, Context context);
    }
}