using System;
using System.Collections.Generic;
using System.IO;
using Navislamia.Maps.Enums;
using static Navislamia.Utilities.StringExtensions;

namespace Navislamia.Maps.Entities;

public class TerrainPropInfo
{
    private List<string> _categoryNames = new ();
    private PropInfo[] _propInfos;

    //TODO configurable?
    private const int MaxPropCount = 65535;

    private const string Nx3Tail = ".nx3";
    private const string NafTail = "_default.naf";
    private const string SptTail = ".spt";
    private const string CobTail = ".cob";
    
    private const string RenderTypeHeader = "RENDERTYPE=";
    private const string CategoryHeader = "CATEGORY=";
    private const string VisibleRatioHeader = "VISIBLE_RATIO=";
    private const string PropNameHeader = "PROPNAME=";

    private const string RenderTypeGeneral = "general";
    private const string RenderTypeBuilding = "building";
    
    public void Initialize(string propInfoFileName)
    {
        Release();

        _propInfos = new PropInfo[MaxPropCount];
        
        foreach (var propInfo in _propInfos)
        {
            propInfo.Type = PropType.UNUSED;
            propInfo.RenderType = RenderType.RENDER_GENERAL;
            propInfo.Category = 0;
            propInfo.VisibleRatio = 1f;
            propInfo.Name = string.Empty;
        }

        if (!File.Exists(propInfoFileName))
        {
            return;
        }

        var textLines = File.ReadAllLines(propInfoFileName);
        var currentRenderType = RenderType.RENDER_GENERAL;
        var currentCategory = 0;
        var currentVisibleRatio = 1f;

        for (var i = 0; i < textLines.Length; i++)
        {
            var line = textLines[i];

            if (line == "")
            {
                continue;
            }

            if (line[0] == ';')
            {
                continue;
            }

            string content;
            if ((content = GetStringContent(line, PropNameHeader)) != null)
            {
                var vString = content.Split(",");

                if (vString.Length <= 1)
                    continue;

                var strPropNum = vString[0];
                var strPropName = vString[1];
                var shadowFlag = (1 << 0) | (1 << 3);

                if (vString.Length == 4)
                {
                    shadowFlag = GetShadowFlag(vString[2]);
                    shadowFlag |= GetShadowFlag(vString[3]);
                }

                if (strPropNum.Length <= 0 || strPropName.Length <= 0)
                {
                    continue;
                }
                
                var propNum = (ushort)int.Parse(strPropNum);

                if (CheckPropFileType(strPropName, Nx3Tail))
                {
                    SetPropInfo(i, propNum, PropType.USE_NX3, currentRenderType, currentCategory,
                        currentVisibleRatio, strPropName, shadowFlag);
                }
                
                if (CheckPropFileType(strPropName, NafTail))
                {
                    SetPropInfo(i, propNum, PropType.USE_NAF, currentRenderType, currentCategory, 
                        currentVisibleRatio, strPropName, shadowFlag);
                }
                
                if (CheckPropFileType(strPropName, SptTail))
                {
                    SetPropInfo(i, propNum, PropType.SPEEDTREE, currentRenderType, currentCategory, 
                        currentVisibleRatio, strPropName, shadowFlag);
                }
                
                if (CheckPropFileType(strPropName, CobTail))
                {
                    SetPropInfo(i, propNum, PropType.NPC, currentRenderType, currentCategory, 
                        currentVisibleRatio, strPropName, shadowFlag);
                }
            }
            else if ((content = GetStringContent(line, CategoryHeader)) != null)
            {
                if (content.Length <= 0)
                {
                    continue;
                }
                
                currentCategory = -1;

                for (var category = 0; category < _categoryNames.Count; category++)
                {
                    if (_categoryNames[category] != content)
                    {
                        continue;
                    }
                    
                    currentCategory = category;
                    break;
                }

                if (currentCategory != -1)
                {
                    continue;
                }
                
                _categoryNames.Add(content);
                currentCategory = _categoryNames.Count - 1;
            }
            
            if ((content = GetStringContent(line, VisibleRatioHeader)) != null)
            {
                currentVisibleRatio = float.Parse(content);

                if (currentVisibleRatio == 0f)
                {
                    currentVisibleRatio = 1f;
                }
            }

            if ((content = GetStringContent(line, RenderTypeHeader)) == null)
            {
                continue;
            }

            currentRenderType = content switch
            {
                RenderTypeGeneral => RenderType.RENDER_GENERAL,
                RenderTypeBuilding => RenderType.RENDER_BUILDING,
                _ => currentRenderType
            };
        }

        if (_categoryNames.Count == 0)
        {
            _categoryNames.Add("?");
        }
    }

    public void Release() => _categoryNames.Clear();

    public List<string> GetCategoryNames() => _categoryNames;

    public PropType GetPropType(ushort propNum) => _propInfos[propNum].Type;

    public int GetShadowFlag(ushort propNum) => _propInfos[propNum].ShadowFlag;

    public RenderType GetPropRenderType(ushort propNum) => _propInfos[propNum].RenderType;

    public bool IsValidProp(ushort propNum) => _propInfos[propNum].Type != PropType.UNUSED;

    public int GetPropCategory(ushort propNum) => IsValidProp(propNum) ? _propInfos[propNum].Category : -1;

    public float GetPropVisibleRatio(ushort propNum) => IsValidProp(propNum) ? _propInfos[propNum].VisibleRatio : 1f;

    public string GetPropName(ushort propNum) => IsValidProp(propNum) ? _propInfos[propNum].Name : "?";

    public string GetPropFileName(ushort propNum, bool noReturnNull = true) => IsValidProp(propNum) ? _propInfos[propNum].Name : noReturnNull ? "" : null;

    public int GetPropLineIndex(ushort propNum) => IsValidProp(propNum) ? _propInfos[propNum].LineIndex : -1;

    public static string GetNx3NameByNafName(string nafFileName)
    {
        var fileName = nafFileName[..^NafTail.Length];

        return $"{fileName}{Nx3Tail}";
    }

    private int GetShadowFlag(string str)
    {
        return str switch
        {
            "no_ca" => 1 << 0,
            "dy_ca" => 1 << 1,
            "st_ca" => 1 << 2,
            "no_re" => 1 << 3,
            "re" => 1 << 4,
            _ => (1 << 0) | (1 << 3)
        };
    }

    private bool CheckPropFileType(string name, string tail)
    {
        var find = name.IndexOf(tail, StringComparison.Ordinal);

        return find != -1 && find == name.Length - tail.Length;
    }

    private void SetPropInfo(int lineIndex, ushort propNum, PropType type, RenderType renderType, int category, float visibleRatio, string name, int shadowFlag)
    {
        _propInfos[propNum].LineIndex = lineIndex;
        _propInfos[propNum].Type = type;
        _propInfos[propNum].RenderType = renderType;
        _propInfos[propNum].Category = category;
        _propInfos[propNum].VisibleRatio = visibleRatio;
        _propInfos[propNum].Name = name;
        _propInfos[propNum].ShadowFlag = shadowFlag;
    }
}
