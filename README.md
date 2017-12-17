# AlexaSkillsKit.NET
.NET library to write Alexa skills that's interface-compatible with [Amazon's AlexaSkillsKit for Java](https://github.com/amzn/alexa-skills-kit-java) and matches that functionality:
* handles the (de)serialization of Alexa requests & responses into easy-to-use object models
* verifies authenticity of the request by validating its signature and timestamp
* code-reviewed and vetted by Amazon (Alexa skills written using this library passed certification)
:new: * supports following interfaces:
** [AudioPlayer](https://developer.amazon.com/docs/custom-skills/audioplayer-interface-reference.html)
** [PlaybackController](https://developer.amazon.com/docs/custom-skills/playback-controller-interface-reference.html)
** [Display](https://developer.amazon.com/docs/custom-skills/display-interface-reference.html)
** [Dialog](https://developer.amazon.com/docs/custom-skills/dialog-interface-reference.html)
** [VideoApp](https://developer.amazon.com/docs/custom-skills/videoapp-interface-reference.html)

Beyond the functionality in Amazon's AlexaSkillsKit for Java, AlexaSkillsKit.NET:
* performs automatic session management so you can easily [build conversational Alexa apps](https://freebusy.io/blog/building-conversational-alexa-apps-for-amazon-echo)
:new: * can be extended to support custom and new coming interfaces (see [Advanced]() section)

This library was originally developed for and is in use at https://freebusy.io

Library is currently available as a single NuGet package: https://www.nuget.org/packages/AlexaSkillsKit.Net/

Extensible version will be available as following NuGet packages:
* https://www.nuget.org/packages/AlexaSkillsKit.Core/ - core library
* https://www.nuget.org/packages/AlexaSkillsKit.Http/ - System.Net.Http provider extentions
* https://www.nuget.org/packages/AlexaSkillsKit.AspNetCore/ - ASP.Net Core Web API provider extentions
* https://www.nuget.org/packages/AlexaSkillsKit.Interfaces.AudioPlayer/ - adds support of AudioPlayer and PlaybackController interfaces
* https://www.nuget.org/packages/AlexaSkillsKit.Interfaces.Dialog/ - adds support of Dialog interface
* https://www.nuget.org/packages/AlexaSkillsKit.Interfaces.Display/ - adds support of Display interface
* https://www.nuget.org/packages/AlexaSkillsKit.Interfaces.VideoApp/ - adds support of VideoApp interface


# How To Use

### 1. Set up your development environment

Read [Getting started with Alexa App development for Amazon Echo using .NET on Windows](https://freebusy.io/blog/getting-started-with-alexa-app-development-for-amazon-echo-using-dot-net)
(only describes .Net Framework 4.x setup)

### 2. Implement your skill as a "Speechlet"

If your Alexa skill does any kind of asynchronous I/O, it's recommended that you derive your app from the abstract `SpeechletAsync` and implement these methods as defined by `ISpeechletAsync`:
  
```csharp
public interface ISpeechletAsync
{
    Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session);
    Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session);
    Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session);
    Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session);
}
```

Alternatively you can derive from synchronous `Speechlet` abstract class and implement these methods as defined by `ISpeechlet`:
  
```csharp
public interface ISpeechlet
{
    SpeechletResponse OnIntent(IntentRequest intentRequest, Session session);
    SpeechletResponse OnLaunch(LaunchRequest launchRequest, Session session);
    void OnSessionStarted(SessionStartedRequest sessionStartedRequest, Session session);
    void OnSessionEnded(SessionEndedRequest sessionEndedRequest, Session session);
}
```
  
Take a look at https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET/blob/master/AlexaSkillsKit.Sample/Speechlet/SampleSessionSpeechlet.cs for an example.

### 3. Wire-up "Speechlet" to HTTP hosting environment

The Sample app is using ASP.NET 4.5 WebApi 2 so wiring-up requests & responses from the HTTP hosting environment (i.e. ASP.NET 4.5) to the "Speechlet" is just a matter of writing a 2-line ApiController like this https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET/blob/master/AlexaSkillsKit.Sample/Speechlet/AlexaController.cs 
  
*Note: sample project is generated from the ASP.NET 4.5 WebApi 2 template so it includes a lot of functionality that's not directly related to Alexa Speechlets, but it does make make for a complete Web API project.*

Alternatively you can host your app and the AlexaSkillsKit.NET library in any other web service framework like ServiceStack, ASP.NET Core Web API, Azure Function App, etc.

# How it works

Starting from version 2.0 AlexaSkillKit provides framework agnostic way to handle incoming Alexa requests.
More over it is refactored, so that Skill authors can now extend library with newly coming not yet natively supported external interfaces and register them in the core library the same way as all existing interfaces are implemented.

Here is detailed explanation of how requests are handled by the library.

### SpeechletService class
To handle Alexa requests an instance of `SpeechletService` has to be created. This is the main entry point for all operations involved in handling incoming requests.
For convenience and backward compatibility with earlier library versions both `Speechlet` and `SpeechletAsync` now have built-in `SpeechletService` instance and wrap all most common operations with it.
Skill authors can access their internal `SpeechletService` through `Service` property.

### Parsing request

When new request is recieved, it first needs to be parced from json string into object model represented by `SpeechletRequestEnvelope` class.
`Task<SpeechletRequestEnvelope> SpeechletService.GetRequestAsync(string content, string chainUrl, string signature)` method is used for this.
Request headers and body validation also takes place during this step. The `SpeechletValidationException` is produced in case of any validation error.

Skill authors can set SpeechletService.ValidationHandler property to have better control on when exception should be thrown, or to throw custom exceptions instead.
For backward compatibility, the `Speechlet` and `SpeechletAsync` helper abstract classes set `ValidationHandler` property of their internal service to virtual `OnRequestValidation` method.
Setting `ValidationHandler` property will override this behavior. See [Override request validation policy](#Override request validation policy) for more details on request validation.
Request validation can be omitted by directly calling one of `SpeechletRequestEnvelope.FromJson` static methods.

`SpeechletRequestEnvelope` consists of `Version`, `Session`, `Context` and `Request` fields. See [Context](#Context) for more details on parsing context.

Only version "1.0" of Alexa Skill API is supported. Otherwise `SpeechletValidationException` with `SpeechletRequestValidationResult.InvalidVersion` is thrown.
Same is true, when calling `SpeechletRequestEnvelope.FromJson` methods directly.

There are a lot of different request types available for Alexa Skills.
Standard requests have simple type names: "LaunchRequest", "IntentRequest", "SessionEndedRequest".
All other requests are related to specific interfaces and their request type name consists of interface name and request subtype name separated by "." sign: "System.ExceptionEncountered", "Dialog.Delegate" and so on.

`SpeechletRequestResolver` static class is used to deserialize `request` json field to appropriate subclass of `SpeechletRequest` base class.
By default, it has no knowledge to which class each request type should be deserialized.
`SpeechletRequestResolver` has `AddInterface` method to bind interface name, with specific `InterfaceResolver`.
`SpeechletRequestResolver` provides `AddStandard` method to register all standard requests and `AddSystem` to register `System.ExceptionEncountered` request.
Both `Speechlet` and `SpeechletAsync` are calling `SpeechletRequestResolver.AddStandard` during initialization.

Each interface library that provide own requests is intended to provide method to register those requests in `SpeechletRequestResolver`.
So do `AlexaSkillsKit.Interfaces.Display` in its `void AddDisplay(this SpeechletService service, IDisplaySpeechletAsync speechlet)` extention method
and `AlexaSkillsKit.Interfaces.AudioPlayer` in `void AddAudioPlayer(this SpeechletService service, IAudioPlayerSpeechletAsync speechlet)`.
For more information on using external interfaces see [Use external interfaces](#Use external interfaces).
For more information on registering custom interfaces see [Implement external interface](#Implement external interface).

### Processing request

//TODO

# Advanced

### Use external interfaces

//TODO

### Implement external interface

//TODO

### Override request validation policy

//TODO

By default, requests with missing or invalid signatures, or with missing or too old timestamps, are rejected. You can override the request validation policy if you'd like not to reject the request in certain conditions and/or to log validation failures.

If you are deriving from SpeechletAsync or Speechlet, simply override `OnRequestValidation` method as follows:

```csharp
/// <summary>
/// return true if you want request to be processed, otherwise false
/// </summary>
public override bool OnRequestValidation(
    SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope) 
{

    if (result != SpeechletRequestValidationResult.OK) 
    {
        if (result.HasFlag(SpeechletRequestValidationResult.NoSignatureHeader)) 
        {
            Debug.WriteLine("Alexa request is missing signature header, rejecting.");
            return false;
        }
        if (result.HasFlag(SpeechletRequestValidationResult.NoCertHeader)) 
        {
            Debug.WriteLine("Alexa request is missing certificate header, rejecting.");
            return false;
        }
        if (result.HasFlag(SpeechletRequestValidationResult.InvalidSignature)) 
        {
            Debug.WriteLine("Alexa request signature is invalid, rejecting.");
            return false;
        }
        else 
        {
            if (result.HasFlag(SpeechletRequestValidationResult.InvalidTimestamp)) 
            {
                var diff = referenceTimeUtc - requestEnvelope.Request.Timestamp;
                Debug.WriteLine("Alexa request timestamped '{0:0.00}' seconds ago making timestamp invalid, but continue processing.",
                    diff.TotalSeconds);
            }
            return true;
        }
    }
    else 
    {      
        var diff = referenceTimeUtc - requestEnvelope.Request.Timestamp;
        Debug.WriteLine("Alexa request timestamped '{0:0.00}' seconds ago.", diff.TotalSeconds);
        return true;
    }            
}
```

### Use Speechlet Service directly

Internally both SpeechletAsync and Speechlet abstract classes are derived from SpeechletBase class. SpeechletBase creates and exposes SpeechletService object used for request handling.

SpeechletService

To have better control over request handling you can use SpeechletService directly or even derive from it.

In this case you need