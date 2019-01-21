using Entia.Messages;
using Entia.Systems;
using UnityEngine;

namespace Entia.Unity.Systems
{
    public struct LogException : IReact<OnException>
    {
        void IReact<OnException>.React(in OnException message) => Debug.LogException(message.Exception);
    }
}
