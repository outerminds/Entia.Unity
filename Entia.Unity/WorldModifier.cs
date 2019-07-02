using UnityEngine;

namespace Entia.Unity
{
    public interface IWorldModifier
    {
        void Modify(World world);
    }

    public abstract class WorldModifier : ScriptableObject, IWorldModifier
    {
        public abstract void Modify(World world);
    }
}