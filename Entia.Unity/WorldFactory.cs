using Entia.Modules;
using Entia.Nodes;
using UnityEngine;

namespace Entia.Unity
{
    public class WorldFactory : ScriptableObject
    {
        public virtual World Create()
        {
            var world = new World();
            world.Builders().Set<Profile>(new Builders.Profile());
            world.Builders().Set<Parallel>(new Builders.Parallel());
            world.Analyzers().Set(new Analyzers.Parallel());
            // world.Templaters().Set(new Templaters.GameObject());
            // world.Templaters().Set(new Templaters.Component());
            // world.Templaters().Set(new Templaters.Transform());
            return world;
        }
    }
}