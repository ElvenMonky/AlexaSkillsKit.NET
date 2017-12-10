namespace AlexaSkillsKit.Speechlet
{
    public static class DisplaySpeechletExtensions
    {
        public static void UseDisplay(this IDisplaySpeechlet speechlet) {
            UseDisplay(new DisplaySpeechletAsyncWrapper(speechlet));
        }

        public static void UseDisplay(this IDisplaySpeechletAsync speechlet) {
            var displayResolver = new InterfaceResolver()
                .WithDefaultDeserializer((json, subtype) => new DisplayRequest(json, subtype));
            SpeechletRequestResolver.UseInterface("Display", displayResolver);

            SpeechletRequestHandler.UseInterface<DisplayRequest>(
                async (request, context) => await speechlet.OnDisplayAsync(request, context));
        }
    }
}