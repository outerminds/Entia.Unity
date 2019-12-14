using System;
using System.Linq;
using Entia.Experimental;
using Entia.Experimental.Templating;
using UnityEngine;

public class Boba : MonoBehaviour
{
    public TemplateReference TemplateA;
    public TemplateReference TemplateB;
    public bool Validate;

    void OnValidate()
    {
        var conflicts = Template.Differentiate(TemplateA?.Template ?? Template.Empty, TemplateB?.Template ?? Template.Empty);
        Debug.Log(string.Join(
            Environment.NewLine,
            conflicts.Select(conflict => $" -> {string.Join(".", conflict.Path)}: ({conflict.ValueA}, {conflict.ValueB})")));
    }
}
