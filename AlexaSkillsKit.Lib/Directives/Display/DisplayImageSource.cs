﻿namespace AlexaSkillsKit.Directives.Display
{
    /// <summary>
    /// https://developer.amazon.com/docs/custom-skills/display-interface-reference.html#image-object-specifications
    /// </summary>
    public class DisplayImageSource
    {
        public virtual string Url {
            get;
            set;
        }

        public virtual ImageSizeEnum Size {
            get;
            set;
        }

        public virtual int WidthPixels {
            get;
            set;
        }

        public virtual int HeightPixels {
            get;
            set;
        }

        public enum ImageSizeEnum
        {
            X_SMALL,
            SMALL,
            MEDIUM,
            LARGE,
            X_LARGE
        }
    }
}