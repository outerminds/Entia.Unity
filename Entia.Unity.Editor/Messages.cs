using Entia.Unity;
using UnityEditor;

namespace Entia.Messages
{
    public struct OnDrawGizmo : IMessage { public GizmoType Type; }
    public struct OnPreInspector<T> : IMessage where T : IReference { public T Reference; public SerializedObject Serialized; }
    public struct OnPreInspector : IMessage { public IReference Reference; public SerializedObject Serialized; }
    public struct OnPostInspector<T> : IMessage where T : IReference { public T Reference; public SerializedObject Serialized; }
    public struct OnPostInspector : IMessage { public IReference Reference; public SerializedObject Serialized; }
    public struct OnValidate<T> : IMessage { public T Reference; }
    public struct OnValidate : IMessage { public IReference Reference; }
}
