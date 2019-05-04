using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entia.Modules;
using Entia.Unity;
using Entia.Core;
using Entia;

public class Poulah : MonoBehaviour
{
    public WorldReference World;
    public EntityReference Entity;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            var templaters = World.World.Templaters();
            var result1 = templaters.Template<Entity>(Entity, Entity.GetType());
            var template = result1.Or(default);
            Debug.Log($"{result1} | {template}");
            var result2 = template.Instantiate();
            var instance = result2.Or(default);
            Debug.Log($"{result2} | {instance}");
        }
    }
}
