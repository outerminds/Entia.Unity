[entia]:https://github.com/outerminds/Entia
[unity]:https://unity3d.com/
[roslyn]:https://en.wikipedia.org/wiki/.NET_Compiler_Platform
[ecs]:https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system
[net-standard]:https://docs.microsoft.com/en-us/dotnet/standard/net-standard
[net-core]:https://dotnet.microsoft.com/download
[logo]:https://github.com/outerminds/Entia/blob/master/Resources/Logo.png
[releases]:https://github.com/outerminds/Entia.Unity/releases
[wiki]:https://github.com/outerminds/Entia.Unity/wiki
[wiki/controller]:https://github.com/outerminds/Entia.Unity/wiki/Controller
[wiki/entity]:https://github.com/outerminds/Entia/wiki/Entity
[wiki/component]:https://github.com/outerminds/Entia/wiki/Component
[wiki/system]:https://github.com/outerminds/Entia/wiki/System
[wiki/resource]:https://github.com/outerminds/Entia/wiki/Resource
[wiki/node]:https://github.com/outerminds/Entia/wiki/Node
[wiki/queryable]:https://github.com/outerminds/Entia/wiki/Queryable
[tutorial/plugins]:https://github.com/outerminds/Entia.Unity/blob/master/Resources/Plugins.PNG
[tutorial/generate]:https://github.com/outerminds/Entia.Unity/blob/master/Resources/Generate.png
[tutorial/generator-settings]:https://github.com/outerminds/Entia.Unity/blob/master/Resources/GeneratorSettings.png
[tutorial/add-component]:https://github.com/outerminds/Entia.Unity/blob/master/Resources/AddComponents.PNG
[tutorial/add-controller]:https://github.com/outerminds/Entia.Unity/blob/master/Resources/AddController.PNG
[tutorial/move-jump]:https://github.com/outerminds/Entia.Unity/blob/master/Resources/MoveJump.gif
# ![Entia.Unity][logo]

**Entia.Unity** is a full integration of the [**Entia**][entia] framework for the Unity game engine. It consists of a code generator, inspectors, templates, tools and other conveniences that make the usage of the framework simple and accessible to Unity developers.

_[**Entia**][entia] is a free, open-source, data-oriented, highly performant, parallelizable and extensible [**E**ntity-**C**omponent-**S**ystem][ecs] (_ECS_) framework written in C# especially for game development. It takes advantage of the latest C#7+ features to represent state exclusively with contiguous structs. No indirection, no boxing, no garbage collection and no cache misses. See [**Entia**][entia]._

#### [:inbox_tray: Download][releases]
#### Entia.Unity requires version 2018.3+ of the [Unity][unity] game engine.
___

### Content
- [Installation](#installation)
- [Tutorial](#tutorial)
- [References](#references)
- [Generator](#generator)
- [Wiki][wiki]
___

# Installation
- [Download][releases] the most recent stable version of **Entia.Unity**.
- Extract the content of the _zip_ file in a _Plugins_ folder in your Unity project (ex: _Project/Assets/Plugins/Entia/_).

![][tutorial/plugins]
- Ensure that you have a _[.Net Core Runtime][net-core]_ with version 2.0+ (required for the code generator to work).
- Optionally install Unity templates by going to the _'Entia'_ menu, then _'Install'_ and '_Templates'_.
- Optionally install the Visual Studio extension _'Entia.Analyze.vsix'_ to get [**Entia**][entia] specific code analysis.
___

# Tutorial
- Create an empty Unity scene.
- In the _'Entia'_ menu, select _'Generator'_ and then _'Generate'_.
  - This will create a `GeneratorSettings` asset in your _'Entia'_ folder and will launch the generator.
  - The default values of the `GeneratorSettings` asset should satisfy most use-cases.
  - As long as the `Automatic` option is on in the `GeneratorSettings`, this is the only time that you will have to manually launch the generator or worry about it.

![][tutorial/generate]

![][tutorial/generator-settings]
- Define a couple [components][wiki/component] in the _'Assets/Scripts'_ folder.
```csharp
using Entia;
using Entia.Unity;

namespace Components
{
    // Components are simple structs that implement the empty 'IComponent' interface.
    public struct Velocity : IComponent { public float X, Y; }

    // Components may be empty to act as a tag on an entity.
    public struct IsFrozen : IComponent { }

    public struct Physics : IComponent
    {
        // Since structs can not have default values, the 'Default' attribute will 
        // cause the generator to generate the default values on the 
        // 'ComponentReference'.
        [Default(1f)]
        public float Mass;
        [Default(3f)]
        public float Drag;
        [Default(-2f)]
        public float Gravity;
    }

    public struct Input : IComponent
    {
        // The 'Disable' attribute will make the field read-only in the Unity editor.
        [Disable]
        public float Direction;
        [Disable]
        public bool Jump;
    }

    public struct Motion : IComponent
    {
        [Default(2f)]
        public float Acceleration;
        [Default(0.25f)]
        public float MaximumSpeed;
        [Default(0.75f)]
        public float JumpForce;
    }
}
```
- Create an empty `GameObject` named _'Player'_, add the newly defined [components][wiki/component] to it and tweak their values (you may addionally add a `SpriteRenderer` to the `GameObject` to visualize it).
  - The generator should've generated `ComponentReference` wrappers for your [components][wiki/component] in a _'Generated'_ folder.
  - You should be able to find them in the _'Add Component'_ dropdown of the `GameObject`.
  - Adding a [component][wiki/component] to a `GameObject` will automatically add the required `EntityReference` to it.

![][tutorial/add-component]
- Define a couple [systems][wiki/system] that will use the [components][wiki/component] in the _'Assets/Scripts'_ folder.
  - When the generator detects the use of a [queryable][wiki/queryable] type, it will generate convenient extensions to unpack instances of that type.
  - Note that you may need to focus Unity to trigger the generator.
```csharp
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Systems
{
    // An 'IRun' system will be called on every 'Update'.
    // A system may implement as many system interfaces as needed.
    // See the 'Entia.Systems' namespace for more system interfaces.
    public struct UpdateInput : IRun
    {
        // Groups allow to efficiently retrieve a subset of entities that 
        // correspond to a given query represented by the generic types of the group.
        // This group will hold every entity that has a 'Components.Input' 
        // component and will give write access to it.
        public readonly Group<Write<Components.Input>> Group;

        public void Run()
        {
            foreach (var item in Group)
            {
                // The 'item.Input()' extension method is generated by the generator.
                ref var input = ref item.Input();
                input.Direction = Input.GetAxis("Horizontal");
                input.Jump = Input.GetKeyDown(KeyCode.UpArrow);
            }
        }
    }

    public struct UpdateVelocity : IRun
    {
        // Queries can also be defined as a separate type which is convenient 
        // for large queries.
        // The 'None' attribute means that all entities that have a 
        // 'Components.IsFrozen' component will be excluded from the query results.
        [None(typeof(Components.IsFrozen))]
        public readonly struct Query : IQueryable
        {
            // A queryable can only hold queryable fields. They can not hold 
            // components directly.
            public readonly Write<Components.Velocity> Velocity;
            public readonly Read<Components.Motion> Motion;
            public readonly Read<Components.Physics> Mass;
            public readonly Read<Components.Input> Input;
        }

        public readonly Group<Query> Group;

        // Resources hold world-global data.
        // 'Entia.Resources.Time' is a library resource that holds 
        // Unity's 'Time.deltaTime'.
        public readonly Resource<Entia.Resources.Time>.Read Time;

        public void Run()
        {
            ref readonly var time = ref Time.Value;
            foreach (ref readonly var item in Group)
            {
                // Unpack the group item using generated extensions.
                ref var velocity = ref item.Velocity();
                ref readonly var motion = ref item.Motion();
                ref readonly var physics = ref item.Physics();
                ref readonly var input = ref item.Input();

                var drag = 1f - physics.Drag * time.Delta;
                velocity.X *= drag;
                velocity.Y *= drag;

                var move = (input.Direction * motion.Acceleration) / physics.Mass;
                velocity.X += move * time.Delta;

                if (input.Jump) velocity.Y += motion.JumpForce / physics.Mass;
                velocity.Y += physics.Gravity * time.Delta;

                // Clamp horizontal velocity.
                if (velocity.X < -motion.MaximumSpeed)
                    velocity.X = -motion.MaximumSpeed;
                if (velocity.X > motion.MaximumSpeed)
                    velocity.X = motion.MaximumSpeed;
            }
        }
    }

    public struct UpdatePosition : IRun
    {
        // 'Unity<Transform>' is a Unity-specific query that gives access 
        // to core Unity components.
        // Note that it will not work for custom 'MonoBehaviour' types.
        public readonly Group<Unity<Transform>, Write<Components.Velocity>> Group;

        public void Run()
        {
            foreach (ref readonly var item in Group)
            {
                // Unpack the group item using generated extensions.
                var transform = item.Transform();
                ref var velocity = ref item.Velocity();

                var position = transform.position;
                position += new Vector3(velocity.X, velocity.Y);

                // Fake a floor.
                if (position.y < 0)
                {
                    position.y = 0;
                    velocity.Y = 0;
                }

                transform.position = position;
            }
        }
    }
}
```
- Define a custom [`ControllerReference`][wiki/controller] and define the execution [node][wiki/node] that will run your [systems][wiki/system].
```csharp
using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;

namespace Controllers
{
    public class Main : ControllerReference
    {
        // This 'Node' represents the execution behavior of systems.
        public override Node Node =>
            // The 'Sequence' node executes its children in order.
            Sequence("TestController",
                // This node holds a few useful Unity-specific library systems.
                Nodes.Default,
                // Any number of systems can be added here.
                System<Systems.UpdateInput>(),
                System<Systems.UpdateVelocity>(),
                System<Systems.UpdatePosition>()
            );
    }
}
```
- Create an empty `GameObject` named '_World_' and add your newly defined controller to it.
  - This will automatically add the required `WorldReference` on the `GameObject`.

![][tutorial/add-controller]

- Press _Play_ and appreciate your moving and jumping player [entity][wiki/entity].

![][tutorial/move-jump]
- For more details, please consult the [wiki][wiki].
___

# References
Most of the integration with the Unity game engine is done through what are called _references_. These are convenient `MonoBehaviour` wrappers that act as constructors and visualizers for [**Entia**][entia] elements. After initialization, references are only debug views for what is going on the [**Entia**][entia] side and are not strictly required. Other than [`ControllerReference`][wiki/controller] (which is where you define your execution graph), you will never have to define references yourself since the code generator will do all the boilerplate work.

# Generator
A lightweight code generator comes packaged with **Entia.Unity** to make the integration with the Unity game engine seamless. It generates corresponding [references](#References) for every [component][wiki/component] and [resource][wiki/resource] that you define such that they can be inspected and adjusted in the editor just like regular `MonoBehaviour` components. Additionally, it will generate convenient extensions for your [systems][wiki/system] to simplify their usage.

Most of the time you will not have to worry about the generator, but it is useful to know that it is triggered when a relevant C# script is saved or imported by the Unity editor. It can also be manually triggered using the menu _'Entia/Generator/Generate'_.

The generator uses [Roslyn][roslyn] to run before the Unity compiler does such that it does not depend on the compiled assembly. This prevents many typical and unpleasant generator bugs (example: _you delete a type and the Unity compiler can't run because some generated code depends on it and since the generator depends on a successful compilation, it can't run either, forcing you to manually modify generated code_).

- The generator is smart enough to detect most renaming scenarios and renames the _.meta_ files associated with the generated `MonoBehaviour` wrappers such that your links will not be lost.
  - Renaming a type through refactor is always detected.
  - Renaming a file is detected.
  - Renaming or changing the `namespace` of a type is detected as long as the file is not renamed at the same time.
- The generator exists only in the Unity editor and will never encumber your builds.
