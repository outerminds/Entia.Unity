[entia]:https://github.com/outerminds/Entia
[unity]:https://unity3d.com/
[roslyn]:https://en.wikipedia.org/wiki/.NET_Compiler_Platform
[ecs]:https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system
[net-standard]:https://docs.microsoft.com/en-us/dotnet/standard/net-standard
[net-core]:https://dotnet.microsoft.com/download
[logo]:https://github.com/outerminds/Entia/blob/master/Resources/Logo.png
[releases]:https://github.com/outerminds/Entia.Unity/releases
[wiki]:https://github.com/outerminds/Entia.Unity/wiki
[wiki/component]:https://github.com/outerminds/Entia/wiki/Component
[wiki/system]:https://github.com/outerminds/Entia/wiki/System
[wiki/resource]:https://github.com/outerminds/Entia/wiki/Resource
[documentation]:./
# ![Entia.Unity][logo]

**Entia.Unity** is a full integration of the [**Entia**][entia] framework for the Unity game engine. It consists of a code generator, inspectors, templates, tools and other conveniences that make the usage of the framework simple and accessible to Unity developers.

_[**Entia**][entia] is a free, open-source, data-oriented, highly performant, parallelizable and extensible [**E**ntity-**C**omponent-**S**ystem (**ECS**)][ecs] framework writtten in C# especially for game development. It takes advantage of the latest C#7+ features to represent state exclusively with contiguous structs. No indirection, no boxing, no garbage collection and no cache misses. See [**Entia**][entia]._

#### [:inbox_tray: Download][releases]
#### Entia.Unity requires version 2018.1+ of the [Unity][unity] game engine.
___

### Content
- [Installation](#installation)
- [References](#references)
- [Generator](#generator)
- [Wiki][wiki]
___

# Installation
- [Download][releases] the most recent stable version of Entia.Unity.
- Extract the content of the _zip_ file in a _Plugins_ folder in your Unity project (ex: _Project/Assets/Plugins/Entia/_).
- Ensure that you have a _[.Net Core Runtime][net-core]_ with version 2.0+ (required for the code generator to work).
- Optionally install the Visual Studio extension _Entia.Analyze.vsix_ to get _Entia_ specific code analysis.
- For more details, please consult the [wiki][wiki].
___

# References
Most of the integration with the Unity game engine is done through what are called _references_. These are convenient _MonoBehaviour_ wrappers that act as contructors and visualizers for **[Entia][entia]** elements. After initialization, references are only debug views for what is going on the **[Entia][entia]** side and are not strictly required. Other than [ControllerReference][wiki/controller] (which is where you define your execution graph), you will never have to define references yourself since the [code generator](#generator) will do all the boilerplate work.

# Generator
A simple code generator comes packaged with **Entia.Unity** to make the integration with the Unity game engine more seemless. It generates corresponding [references](#References) for every [component][wiki/component] and [resource][wiki/resource] that you define such that they can be inspected and adjusted in the editor just like regular _MonoBehaviour_ components. Additionally, it will generate convenient extensions for your [systems][wiki/system] to simplify their usage.

Most of the time you will not have to worry about the generator, but it is useful to know that it is triggered when a relevant C# script is imported by the Unity editor. It can also be manually triggered using the menu _Entia/Generator/Generate_.

The generator uses [Roslyn][roslyn] to run before the Unity compiler does such that it does not depend on the compiled assembly. This prevents many typical and unpleasant generator bugs (example: _you delete a type and the Unity compiler can't run because some generated code depends on it and since the generator depends on a successful compilation, it can't run either, forcing you to manually modify generated code_).

- The generator is smart enough to detect most renaming scenarios and renames the _.meta_ files associated with the generated _MonoBehaviour_ wrappers such that your links will not be lost.
  - Renaming a type through refactor is always detected.
  - Renaming a file is detected.
  - Renaming or changing the `namespace` of a type is detected as long as the file is not renamed at the same time.
- The generator exists only in the Unity editor and will never encumber your builds._
