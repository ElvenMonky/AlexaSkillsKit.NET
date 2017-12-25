﻿using AlexaSkillsKit.Json;
using AlexaSkillsKit.Speechlet;

namespace AlexaSkillsKit.Interfaces.AudioPlayer
{
    public static class AudioPlayerSpeechletServiceExtensions
    {
        public static void AddAudioPlayer(this SpeechletService service, IAudioPlayerSpeechlet speechlet) {
            service.AddAudioPlayer(new AudioPlayerSpeechletAsyncWrapper(speechlet));
        }

        public static void AddAudioPlayer(this SpeechletService service, IAudioPlayerSpeechletAsync speechlet) {
            Deserializer<ISpeechletInterface>.RegisterDeserializer("AudioPlayer", AudioPlayerInterface.FromJson);

            SpeechletRequestEnvelope.RequestParser.AddInterface("AudioPlayer", (subtype, json) => {
                switch (subtype) {
                    case "PlaybackFailed":
                        return new AudioPlayerPlaybackFailedRequest(subtype, json);
                    default:
                        return new AudioPlayerRequest(subtype, json);
                }
            });

            service.AddHandler<AudioPlayerRequest>(
                async (request, context) => await speechlet.OnAudioPlayerAsync(request, context));

            SpeechletRequestEnvelope.RequestParser.AddInterface("PlaybackController", (subtype, json) => new PlaybackControllerRequest(subtype, json));

            service.AddHandler<PlaybackControllerRequest>(
                async (request, context) => await speechlet.OnPlaybackControllerAsync(request, context));
        }
    }
}