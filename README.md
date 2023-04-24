# Navislamia
---
[![Build Status](https://github.com/iSmokeDrow/Navislamia/actions/workflows/build.yml/badge.svg?branch=Development)](https://github.com/iSmokeDrow/Navislamia/actions/workflows/build.yml)
[![OpenRZ Discord](https://badgen.net/discord/members/UQz9uydsFY)](https://discord.gg/UQz9uydsFY)

[![Recent Commits](https://img.shields.io/github/commit-activity/m/iSmokeDrow/Navislamia?label=Commits&style=flat-square)]()
[![Open Pull Requests](https://img.shields.io/github/issues-pr-raw/ismokedrow/navislamia?label=Open%20Pull%20Requests&style=flat-square)]()
[![Open Issues](https://img.shields.io/github/issues-raw/ismokedrow/navislamia?color=red&label=Open%20Issues&style=flat-square)]()
[![Closed Issues](https://img.shields.io/github/issues-closed-raw/ismokedrow/navislamia?color=Green&label=Closed%20Issues&style=flat-square)]()

## Foreward

Navislamia is written and maintained by a group of volunteers for fun. This project does not now, nor will it ever seek monetary reimbursement or gains. It is not our intent to defame or otherwise denigrate the hard work done by developers of the `Arcadia Framework`. More so it is our intent to pay homage to the game all the volunteers and players involved have sunk countless hours and years of their lives into.

## **What is Navislamia?** 

An open source `.NET 5` reimplementation of the `Arcadia Framework` utilized by the `CaptainHerlockServer` executable, written in `CSharp 9`

## **Why is Navislamia?**

While some `emulator` projects have been started and orphaned, there still has yet to be a simple common sense reimplementation of the `Arcadia Framework`. Let alone a production ready example of a `CaptainHerlockServer` executable replacement. At OpenRZ our volunteer developers aims to change the status quo by delivering not only a fully featured and production ready `Arcadia Framework` reimplementation, but, a fully featured `CaptainHerlockServer` executable reimplementation to demonstrate the power of the new framework.

## Key Features
---

### Light weight, Purpose Driven

In this reimplementation you will not find code repetition or crowding, we firmly believe that code should be short and concise. It should speak for itself in other words. It is in this spirit that the Navislamia solution is broken down into a small, extensible and purpose driven set of modules

### Built on D.I.P (The Dependency Injection Principle)

Harnessing the vast and powerful improvements made to the `.NET framework` Navislamia uses abstraction ontop of an easily configured dependency container to work magic behind the scenes in regards to module dependency scope and lifetime management. This vastly simplifies construction calls to registered module classes and overall gives the code a higher degree of readability while shifting the way dependencies are called and used. Utimately allowing modules to be changed with little to no effect on the calling application.

### Task Oriented

Threading has been and continues to be a bane in the side of many `emulator` frameworks. This can be seen in the operation of the vanilla `Arcadia Framework`. Navislamia however approaches this issue through the [Microsoft Task API](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks?view=net-6.0) allowing us to offload the responsibilities of thread creating/pooling and marshaling. While also giving us the flexibility to make asset loads in `parallel` with one another in most casts. Drastically reducing processing times.

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

If you feel like you or someone you know would like to contribute to this project, please click the discord link above and introduce yourself in the lobby.


