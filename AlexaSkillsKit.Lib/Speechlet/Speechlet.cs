﻿//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;

namespace AlexaSkillsKit.Speechlet
{
    [Obsolete("Does not support context object. Derive from SpeechletBase instead and implement ISpeechletV2")]
    public abstract class Speechlet : SpeechletBase, ISpeechlet
    {
        public abstract SpeechletResponse OnIntent(IntentRequest intentRequest, Session session);
        public abstract SpeechletResponse OnLaunch(LaunchRequest launchRequest, Session session);
        public abstract void OnSessionStarted(SessionStartedRequest sessionStartedRequest, Session session);
        public abstract void OnSessionEnded(SessionEndedRequest sessionEndedRequest, Session session);
    }
}