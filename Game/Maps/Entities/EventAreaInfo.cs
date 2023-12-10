using Navislamia.Game.Maps.Constants;
using Navislamia.Game.Maps.X2D;

namespace Navislamia.Game.Maps.Entities;

public class EventAreaInfo
{
    public int Id;

    public static bool IsActivatable(/*StructPlayer pPlayer, int areaIndex*/) => false; // TODO: implement EventAreaInfo.IsActivatable

    public PolygonF Area;

    public int BeginTime;
    public int EndTime;
    public int MinLevel;
    public int MaxLevel;

    public long RaceJobLimit;

    public int[] ActivateCondition = new int[EventAreaInfoConstants.MaxActivateConditions];
    public int[][] ActivateValue = new int[EventAreaInfoConstants.MaxActivateConditions][];
    public int LimitActivateCount;

    public string EnterHandler;
    public string LeaveHandler;
    
    public EventAreaInfo(int id, PointF[] points)
    {
        Id = id;
        Area = new PolygonF(points);

        BeginTime = 0;
        EndTime = 0;
        MinLevel = 0;
        MaxLevel = 0;
        RaceJobLimit = 0;
        LimitActivateCount = 0;
        EnterHandler = "";
        LeaveHandler = "";

        for (var index = 0; index < EventAreaInfoConstants.MaxActivateConditions; ++index)
        {
            ActivateCondition[index] = 0;
            ActivateValue[index] = new []{ 0, 0 };
        }
    }


}
