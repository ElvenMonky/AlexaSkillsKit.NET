﻿namespace AlexaSkillsKit.Directives
{
    public class Directive
    {
        public Directive(string type) {
            Type = type;
        }

        public virtual string Type {
            get;
            private set;
        }
    }
}