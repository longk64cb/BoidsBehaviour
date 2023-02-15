using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIContext
{
    private int size = 16;
    public int Size => size;

    private List<float> weights = new List<float>();
    public float factor = 1f;

    private float minValue;
    private float maxValue;

    public AIContext(int size)
    {
        this.size = size;
        weights.Clear();
        for (int i = 0; i < size; i++)
        {
            weights.Add(0);
        }
    }

    public void SetWeight(int index, float value)
    {
        if (index < size && index >= 0) weights[index] = value;
    }

    public float GetWeight(int index)
    {
        if (index < size && index >= 0) return weights[index] * factor;
        return -Mathf.Infinity;
    }

    public void ClearWeight()
    {
        for (int idx = 0; idx < size; idx++)
        {
            weights[idx] = 0;
        }
    }

    public void Combine(AIContext[] contexts)
    {
        for (int i = 0; i < size; i++)
        {
            weights[i] = 0f;
            foreach (AIContext context in contexts)
            {
                weights[i] += context.GetWeight(i);
            }
        }
    }

    public Vector2 GetDesiredDirection()
    {
        int maxIndex = 0;
        int minIndex = 0;

         maxValue = GetWeight(0);
         minValue = GetWeight(0);
        for (int i = 0; i < size; i++)
        {
            if (GetWeight(i) > maxValue)
            {
                maxIndex = i;
                maxValue = GetWeight(i);
            }

            if (GetWeight(i) < minValue)
            {
                minIndex = i;
                minValue = GetWeight(i);
            }
        }
        //NormalizeWeights();

        return Directions.detectDirections[maxIndex];
    }

    public Vector2 GetAvoidDirection()
    {
        int minIndex = 0;
        float minValue = weights[0];
        for (int i = 0; i < size; i++)
        {
            if (weights[i] < minValue)
            {
                minIndex = i;
                minValue = weights[i];
            }
        }

        return Directions.detectDirections[minIndex];
    }

    public void NormalizeWeights()
    {
        for (int i = 0; i < size; i++)
        {
            weights[i] = Utilities.MapValue(weights[i], minValue, maxValue);
        }
    }
}
