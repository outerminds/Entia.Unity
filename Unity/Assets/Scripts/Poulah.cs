using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entia.Modules;
using Entia.Unity;
using Entia.Core;

public class Poulah : MonoBehaviour
{
    public WorldReference World;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            var template = World.World.Templaters().Template(this).Or(default);
            var result = template.Instantiate().Or(default);
            Debug.Log($"{result} | {this} | {this == result}");
        }
    }
}
