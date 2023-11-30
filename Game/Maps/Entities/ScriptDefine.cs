using Navislamia.Maps.Enums;

namespace Navislamia.Maps.Entities;

public static class ScriptDefine
{

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
