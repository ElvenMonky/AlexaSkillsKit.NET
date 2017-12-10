﻿using System.Threading.Tasks;

namespace AlexaSkillsKit.Speechlet
{
    public class DisplaySpeechletAsyncWrapper : IDisplaySpeechletAsync
    {
        private readonly IDisplaySpeechlet speechlet;

        public DisplaySpeechletAsyncWrapper(IDisplaySpeechlet speechlet) {
            this.speechlet = speechlet;
        }

        public async Task<SpeechletResponse> OnDisplayAsync(DisplayRequest displayRequest, Context context) {
            return speechlet.OnDisplay(displayRequest, context);
        }
    }
}