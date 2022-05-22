using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Utilities.StringExt;

namespace Maps
{
    public class TerrainPropInfo
    {
        public bool Initialize(string propInfoFileName)
        {
            Release();

            propInfo = new PropInfo[maxPropCount];
            
            for (int i = 0; i < propInfo.Length; ++i)
            {
                propInfo[i].Type = PropType.UNUSED;
                propInfo[i].RenderType = RenderType.RENDER_GENERAL;
                propInfo[i].Category = 0;
                propInfo[i].VisibleRatio = 1f;
                propInfo[i].Name = string.Empty;
            }

            if (!File.Exists(propInfoFileName))
                return false;

            string[] textLines = File.ReadAllLines(propInfoFileName);

            const string renderTypeHeader = "RENDERTYPE=";
            const string categoryHeader = "CATEGORY=";
            const string visibleRatioHeader = "VISIBLE_RATIO=";
            const string propNameHeader = "PROPNAME=";

            const string renderTypeGeneral = "general";
            const string renderTypeBuilding = "building";

            RenderType currentRenderType = RenderType.RENDER_GENERAL;
            int currentCategory = 0;
            float currentVisibleRatio = 1f;

            for (int i = 0; i < textLines.Length; i++)
            {
                string line = textLines[i];
                string content = null;

                if (line == "")
                    continue;

                if (line[0] == ';')
                    continue;

                if ((content = GetStringContent(line, propNameHeader)) != null)
                {
                    string[] vString = content.Split(",");

                    if (vString.Length <= 1)
                        continue;

                    string strPropNum = vString[0];
                    string strPropName = vString[1];
                    int shadowFlag = (1 << 0) | (1 << 3);

                    if (vString.Length == 4)
                    {
                        shadowFlag = getShadowFlag(vString[2]);
                        shadowFlag |= getShadowFlag(vString[3]);
                    }

                    if (strPropNum.Length > 0 && strPropName.Length > 0)
                    {
                        ushort propNum = (ushort)int.Parse(strPropNum);

                        if (checkPropFileType(strPropName, nx3Tail))
                            setPropInfo(i, propNum, PropType.USE_NX3, currentRenderType, currentCategory, currentVisibleRatio, strPropName, shadowFlag);
                        else if (checkPropFileType(strPropName, nafTail))
                            setPropInfo(i, propNum, PropType.USE_NAF, currentRenderType, currentCategory, currentVisibleRatio, strPropName, shadowFlag);
                        else if (checkPropFileType(strPropName, sptTail))
                            setPropInfo(i, propNum, PropType.SPEEDTREE, currentRenderType, currentCategory, currentVisibleRatio, strPropName, shadowFlag);
                        else if (checkPropFileType(strPropName, cobTail))
                            setPropInfo(i, propNum, PropType.NPC, currentRenderType, currentCategory, currentVisibleRatio, strPropName, shadowFlag);
                    }
                }
                else if ((content = GetStringContent(line, categoryHeader)) != null)
                {
                    if (content.Length > 0)
                    {
                        currentCategory = -1;

                        for (int category = 0; category < categoryNames.Count; category++)
                        {
                            if (categoryNames[category] == content)
                            {
                                currentCategory = category;
                                break;
                            }
                        }

                        if (currentCategory == -1)
                        {
                            categoryNames.Add(content);
                            currentCategory = categoryNames.Count - 1;
                        }
                    }
                }
                else if ((content = GetStringContent(line, visibleRatioHeader)) != null)
                {
                    currentVisibleRatio = float.Parse(content);

                    if (currentVisibleRatio == 0f)
                        currentVisibleRatio = 1f;
                }
                else if ((content = GetStringContent(line, renderTypeHeader)) != null)
                {
                    if (content == renderTypeGeneral)
                        currentRenderType = RenderType.RENDER_GENERAL;
                    else if (content == renderTypeBuilding)
                        currentRenderType = RenderType.RENDER_BUILDING;
                }
            }

            if (categoryNames.Count == 0)
                categoryNames.Add("?");

            return true;
        }

        public void Release() => categoryNames.Clear();

        public List<string> GetCategoryNames() => categoryNames;

        public PropType GetPropType(ushort propNum) => propInfo[propNum].Type;

        public int GetShadowFlag(ushort propNum) => propInfo[propNum].ShadowFlag;

        public RenderType GetPropRenderType(ushort propNum) => propInfo[propNum].RenderType;

        public bool IsValidProp(ushort propNum) => propInfo[propNum].Type != PropType.UNUSED;

        public int GetPropCategory(ushort propNum) => (IsValidProp(propNum)) ? propInfo[propNum].Category : -1;

        public float GetPropVisibleRatio(ushort propNum) => (IsValidProp(propNum)) ? propInfo[propNum].VisibleRatio : 1f;

        public string GetPropName(ushort propNum) => (IsValidProp(propNum)) ? propInfo[propNum].Name : "?";

        public string GetPropFileName(ushort propNum, bool noReturnNull = true) => (IsValidProp(propNum)) ? propInfo[propNum].Name : (noReturnNull) ? "" : null;

        public int GetPropLineIndex(ushort propNum) => (IsValidProp(propNum)) ? propInfo[propNum].LineIndex : -1;

        public static string GetNX3NameByNAFName(string nafFileName)
        {
            string fileName;
            fileName = nafFileName.Substring(0, nafFileName.Length - nafTail.Length);

            return $"{fileName}{nx3Tail}";
        }

        int getShadowFlag(string str)
        {
            switch (str)
            {
                case "no_ca":
                    return 1 << 0;

                case "dy_ca":
                    return 1 << 1;

                case "st_ca":
                    return 1 << 2;

                case "no_re":
                    return 1 << 3;

                case "re":
                    return 1 << 4;

                default:
                    return (1 << 0) | (1 << 3);
            }
        }

        bool checkPropFileType(string name, string tail)
        {
            int find = name.IndexOf(tail);

            return (find != -1) ? (find == name.Length - tail.Length) : false;
        }

        void setPropInfo(int lineIndex, ushort propNum, PropType type, RenderType renderType, int category, float visibleRatio, string name, int shadowFlag)
        {
            propInfo[propNum].LineIndex = lineIndex;
            propInfo[propNum].Type = type;
            propInfo[propNum].RenderType = renderType;
            propInfo[propNum].Category = category;
            propInfo[propNum].VisibleRatio = visibleRatio;
            propInfo[propNum].Name = name;
            propInfo[propNum].ShadowFlag = shadowFlag;
        }

        List<string> categoryNames = new List<string>();
        PropInfo[] propInfo = null;

        const int maxPropCount = 65535;

        const string nx3Tail = ".nx3";
        const string nafTail = "_default.naf";
        const string sptTail = ".spt";
        const string cobTail = ".cob";
    }

    public enum PropType
    {
        UNUSED,
        USE_NAF,
        USE_NX3,
        SPEEDTREE,
        NPC
    }

    public enum RenderType
    {
        RENDER_GENERAL,
        RENDER_BUILDING
    }

    public struct PropInfo
    {
        public int LineIndex;
        public PropType Type;
        public RenderType RenderType;
        public int Category;
        public float VisibleRatio;
        public int ShadowFlag;
        public string Name;
    }
}
