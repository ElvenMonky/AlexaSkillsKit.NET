using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.Display.Directives
{
    public static class SpeechletResponseBuilderExtentions
    {
        public static ISpeechletResponseBuilder WithDisplayRenderTemplateDirective(this ISpeechletResponseBuilder builder,
            DisplayTemplate template) {
            return builder.KeepSession().WithDirective(new DisplayRenderTemplateDirective {
                Template = template
            });
        }
    }
}