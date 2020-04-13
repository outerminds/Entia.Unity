using UnityEngine;

namespace Entia.Experimental.Unity
{
    public abstract class WorldModifier : ScriptableObject
    {
        public abstract void Modify(World world);
    }
}