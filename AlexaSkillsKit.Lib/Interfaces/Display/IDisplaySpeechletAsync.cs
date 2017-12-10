using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public interface IDisplaySpeechletAsync
    {
        Task<SpeechletResponse> OnDisplayAsync(DisplayRequest displayRequest, Context context);
    }
}