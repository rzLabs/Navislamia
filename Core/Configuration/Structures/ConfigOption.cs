using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Navislamia.Configuration.Structures
{
    [DebuggerDisplay("{Name}", Name = "Name")]
    public class Setting
    {
        public string Parent;
        public string Name;
        public Type Type;
        public dynamic Value;
        public List<string> Comments;
    }
}