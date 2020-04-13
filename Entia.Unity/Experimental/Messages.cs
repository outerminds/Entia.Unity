namespace Entia.Experimental.Phases
{
    public readonly struct OnInitialize : IMessage { }
    public readonly struct OnDispose : IMessage { }
    public readonly struct OnAwake : IMessage { }
    public readonly struct OnDestroy : IMessage { }
    public readonly struct OnStart : IMessage { }
    public readonly struct OnEnable : IMessage { }
    public readonly struct OnDisable : IMessage { }
    public readonly struct OnUpdate : IMessage { }
    public readonly struct OnFixedUpdate : IMessage { }
    public readonly struct OnLateUpdate : IMessage { }
}