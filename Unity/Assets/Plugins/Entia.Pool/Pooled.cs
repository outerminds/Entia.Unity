using Entia;
using Entia.Core;
using Entia.Delegables;
using Entia.Dependables;
using Entia.Unity;
using UnityEngine;

namespace Entia.Components
{
    [Plain]
    public struct Pooled : IComponent
    {
        public EntityReference Reference;
        public GameObject Instance;
    }
}