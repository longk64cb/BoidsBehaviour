using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelRange
{
    None,
    L10,
    L20,
    L40,
    L60,
    L80,
    L100,
    L140,
    L180,
    L220,
    L260,
    L300,
    LMax,
}

public class CheckLevelRange
{
    public static float[] param1 = new[]
    {
        0.2500f, 0.1800f, 0.1200f,
        0.0900f, 0.0700f, 0.0500f, 
        0.0400f, 0.0250f,
        0.020001f, 0.0150f, 0.01001f
    };
    
    public static float[] param2 = new[]
    {
        0.2500f, 0.1800f, 0.1200f,
        0.0900f, 0.0700f, 0.0520f,
        0.0420f, 0.0320f, 0.0260f, 
        0.0200f, 0.0150f, 0.0100f
    };
    public static LevelRange CheckLevel(int level)
    {
        if (level <= 10)
        {
            return LevelRange.L10;
        }

        if (level > 10 && level <= 20)
        {
            return LevelRange.L20;
        }

        if (level > 20 && level <= 40)
        {
            return LevelRange.L40;
        }

        if (level > 40 && level <= 60)
        {
            return LevelRange.L60;
        }

        if (level > 60 && level <= 80)
        {
            return LevelRange.L80;
        }

        if (level > 80 && level <= 100)
        {
            return LevelRange.L100;
        }

        if (level > 100 && level <= 140)
        {
            return LevelRange.L140;
        }

        if (level > 140 && level <= 180)
        {
            return LevelRange.L180;
        }

        if (level > 180 && level <= 220)
        {
            return LevelRange.L220;
        }

        if (level > 220 && level <= 260)
        {
            return LevelRange.L260;
        }

        if (level > 260 && level <= 300)
        {
            return LevelRange.L300;
        }

        if (level > 300)
        {
            return LevelRange.LMax;
        }

        return LevelRange.None;
    }
}