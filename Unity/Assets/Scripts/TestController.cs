using Entia;
using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;
using System.Collections;
using System.Collections.Generic;

namespace Controllers
{
    public class TestController : ControllerReference
    {
        public override Node Node =>
            Sequence("TestController",
                Nodes.Default
            );
    }
}