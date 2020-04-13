using UnityEngine.SceneManagement;

namespace Entia.Experimental.Phases
{
    public struct OnLoad : IMessage { public Scene Scene; }
    public struct OnUnload : IMessage { public Scene Scene; }
    public readonly struct OnInitialize : IMessage { }
    public readonly struct OnDispose : IMessage { }
    public readonly struct OnQuit : IMessage { }
    public readonly struct OnUpdate : IMessage { }
    public readonly struct OnFixedUpdate : IMessage { }
    public readonly struct OnLateUpdate : IMessage { }
}