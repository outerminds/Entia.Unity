using System.Collections.Generic;
using Entia.Core;
using Entia.Delegates;
using Entia.Initializers;
using Entia.Instantiators;
using Entia.Modules.Template;
using UnityEngine;

namespace Entia.Templaters
{
    // public sealed class EntityReference : ITemplater
    // {
    //     public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
    //     {
    //         if (context.Index == 0 && context.Value is Unity.EntityReference reference)
    //         {
    //             var components = reference.GetComponentsInChildren<UnityEngine.Component>();
    //         }
    //         else
    //             return (new Instantiators.Constant(context.Value), new Initializers.Identity());
    //     }
    // }
}