﻿namespace AlexaSkillsKit.Directives.VideoApp
{
    /// <summary>
    /// https://developer.amazon.com/docs/custom-skills/videoapp-interface-reference.html#videoapp-directives
    /// </summary>
    public class VideoAppLaunchDirective : Directive
    {
        public VideoAppLaunchDirective() : base("VideoApp.Launch") {

        }

        public virtual VideoItem VideoItem {
            get;
            set;
        }
    }
}