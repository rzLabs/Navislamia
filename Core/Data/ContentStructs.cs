using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data
{
    public struct PropContactScriptInfo
    {
        public int PropID;
        public float X, Y;
        public int Prop_Type;
        public int Model_Info;
        public List<_FunctionList> FunctionList;

        public enum PropType
        {
            NPC,
            Prop
        }

        public struct _FunctionList
        {
            public enum TriggerType
            {
                INIT,
                CONTACT
            }

            public int TriggerID;
            public string Function;
        }
    }
}
