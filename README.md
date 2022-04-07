# Navislamia
---

## Foreward

Navislamia is written and maintained by a group of volunteers for fun. This project does not now, nor will it ever seek monetary reimbursement or gains. It is not our intent to defame or otherwise denigrate the hard work done by developers of the `Arcadia Framework`. More so it is our intent to pay homage to the game all the volunteers and players involved have sunk countless hours and years of their lives into.

## **What is Navislamia?** 

An open source `.NET 5` reimplementation of the `Arcadia Framework` utilized by the `CaptainHerlockServer` executable, written in `CSharp 9`

## **Why is Navislamia?**

While some `emulator` projects have been started and orphaned, there still has yet to be a simple common sense reimplementation of the `Arcadia Framework`. The Navislamia project aims to create simple and powerful set of extensible `Modules` that can be used in a number of future endeavours, creating a reimplementation of the `CaptainHerlockServer` executable being only one example.

## Key Features
---

### Light weight, Purpose Driven

No one likes code repetition right? Well, except for the developers at `Gala-Labs` Navislamia takes a different approach. Sequestering its code out into well organized and purpose driven `Modules` that use the `Module Subscription Model` *(as I have dubbed it)*

### Module Subscription Model

Navislamia as a solution is comprised of several (generally smaller) `Module` projects like `GameModule, ScriptModule` etc.

Implementing the [Dependency Inversion Principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle) each `Module` is required to expose a constructor that accepts a `List<object>` of dependencies to be used by it.

A common sense use of this model can be seen in the example below:

```csharp
public class Example
{
    IConfigurationService configService = new ConfigurationModule();
    INotificationService notificationService = new NotificationModule();
    IDatabaseService databaseService = new DatabaseModule(new List<object>(){ configService, notificationService });
}
```

### Task Oriented

Threading can be a nightmare, especially in languages like `c++`. Thankfully in our lovely managed universe of `c#` we can offload this responsbility to the wonderful [Microsoft Task API](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks?view=net-6.0). Doing so allows Navislamia to execute many traditionally synchronous *(linear)* operations in `parallel` *(at the same time)*. An example of this is loading assets side by side as long as they do not depend on one another.

## Community
---

### How can I help?

A project of this scope requires varying talent to progress to completion. That being said the project is currently seeking:

- Git maintainer (help w/ readme and wiki documentation)
- Developers proficient in:
    - .NET, ASP.NET Web Development, MSSQL, PostgreSQL
    - LUA 5.2 Scripting
    - Unity, Graphics Design and Animations
    - Community Management

If you feel like you or someone you know would like to contribute to this project, please click the discord link below and introduce yourself in the lobby.

### [Discord](https://discord.gg/73mGPjr)

## Build Status

[![Build Solution](https://github.com/iSmokeDrow/Navislamia/actions/workflows/build.yml/badge.svg?branch=Development)](https://github.com/iSmokeDrow/Navislamia/actions/workflows/build.yml)  


