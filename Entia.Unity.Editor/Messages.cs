using Entia.Unity;
using UnityEditor;

namespace Entia.Messages
{
    public struct OnDrawGizmo : IMessage { public GizmoType Type; }
    public struct OnInspector<T> : IMessage where T : IReference { public T Reference; public SerializedObject Serialized; }
    public struct OnInspector : IMessage { public IReference Reference; public SerializedObject Serialized; }
    public struct OnValidate<T> : IMessage { public T Reference; }
    public struct OnValidate : IMessage { public IReference Reference; }
}
