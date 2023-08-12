using System;
using UnityEngine;
public class Settings
{
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;
    
    //时间相关
    public const float secondThreshold = 0.01f;//数值越小时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;
    public const int seasonHold = 3;
    
    
    //Transition
    public const float fadeDuration = 1.5f;
    
    
    //NPC网格偏移
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;
    
    public const float animationBreakTime = 5f;
    public const int maxGridSize = 9999;
    
    //灯光
    public const float lightChangeDuration = 25f;
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    public static Vector3 playerStartPos = new Vector3(-1.7f, -5f, 0);
    public const int playerStartMoney = 100000;

}
