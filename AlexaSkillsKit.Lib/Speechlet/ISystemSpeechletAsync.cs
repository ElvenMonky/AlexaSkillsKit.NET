using AlexaSkillsKit.Requests;
using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public interface ISystemSpeechletAsync
    {
        Task OnSystemExceptionEncounteredAsync(SystemExceptionEncounteredRequest systemRequest, Context context);
    }
}