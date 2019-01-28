using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Entia.Core;
using UnityEngine;

public unsafe class Testy : MonoBehaviour
{
    public bool Run;

    void OnValidate()
    {
        if (Run)
        {
            Run = false;
            var vector3 = new Vector3(1f, 2f, 3f);
            var vector2 = new Vector2();
            UnsafeUtility.Reinterpret.Value(ref vector3, ref vector2);

            var fields = typeof(TypedReference).GetFields(TypeUtility.Instance);
            var index = Array.FindIndex(fields, field => field.Name == "Value");
            var offset = index * 8;
            var source = __makeref(vector3);
            var target = __makeref(vector2);
            var sourcePointer = (IntPtr*)&source;
            var targetPointer = (IntPtr*)&target;
            targetPointer[index] = sourcePointer[index];
            var value1 = __refvalue(source, Vector3);
            var value2 = __refvalue(target, Vector2);

            Debug.Log($"{vector3} -> {vector2} -> {value1} -> {value2}");
        }
    }
}
