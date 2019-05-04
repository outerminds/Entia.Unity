using Entia.Core;
using Entia.Initializers;
using Entia.Instantiators;
using Entia.Modules.Template;

namespace Entia.Templaters
{
    public sealed class Object : ITemplater
    {
        public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
        {
            if (context.Index == 0 && context.Value is UnityEngine.Object @object)
                return (new UnityInstantiate(@object), new Identity());
            else
                return (new Constant(context.Value), new Identity());
        }
    }
}