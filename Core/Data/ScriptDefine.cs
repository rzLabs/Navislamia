using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data
{
    public static class ScriptDefine
    {
        public const string NFSFILE_SIGN = "nFlavor Script";
        public const int NFSCurrentVer = 2;

        public struct NFS_HEADER_V02
        {
            /// <summary>
            /// 16 Bytes long
            /// </summary>
            public string Sign;

            public int Version;
            public int EventLocationOffset;
            public int EventScriptOffset;
            public int PropScriptOffset;
        }

        public enum ScriptTrigger
        {
            Initialize,
            Contact,
            Enter,
            Leave,
            ClientWater,
            MinimapInfo
        }

        public struct ScriptFunction
        {
            public ScriptTrigger Trigger;
            public string FuncName;

            public ScriptFunction(ScriptTrigger trigger, string funcName)
            {
                Trigger = trigger;
                FuncName = funcName;
            }
        }
    }
}
