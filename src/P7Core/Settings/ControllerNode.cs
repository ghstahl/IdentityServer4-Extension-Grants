using System.Collections.Generic;
using P7Core.Settings;

namespace P7Core.Settings
{
    public class ControllerNode
    {
        public string Controller { get; set; }
        public List<ActionNode> Actions { get; set; }
    }
}