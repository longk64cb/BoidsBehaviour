using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static float MapValue(float value, float minValue, float maxValue)
    {
        float map = (value - minValue) / (maxValue - minValue);
        return Mathf.Clamp01(map);
    }
}
