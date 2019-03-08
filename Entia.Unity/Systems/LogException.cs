using Entia.Messages;
using Entia.Systems;
using UnityEngine;

namespace Entia.Unity.Systems
{
    public readonly struct LogException : IReact<OnException>
    {
        void IReact<OnException>.React(in OnException message) => Debug.LogException(message.Exception);
    }
}
