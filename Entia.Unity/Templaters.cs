using Entia.Core;
using Entia.Initializers;
using Entia.Instantiators;
using Entia.Modules.Template;
using UnityEngine;

namespace Entia.Templaters
{
    public sealed class Object : ITemplater
    {
        public sealed class Instantiator : Instantiator<UnityEngine.Object>
        {
            public readonly UnityEngine.Object Object;
            public Instantiator(UnityEngine.Object @object) { Object = @object; }
            public override Result<UnityEngine.Object> Instantiate(object[] instances) => UnityEngine.Object.Instantiate(Object);
        }

        public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
        {
            if (context.Index == 0 && context.Value is UnityEngine.Object @object)
                return (new Instantiator(@object), new Initializers.Identity());
            else
                return (new Instantiators.Constant(context.Value), new Initializers.Identity());
        }
    }
}