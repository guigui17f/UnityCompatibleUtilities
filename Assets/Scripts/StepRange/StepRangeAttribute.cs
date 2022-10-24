using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class StepRangeAttribute : PropertyAttribute
{
    public readonly float Min;
    public readonly float Max;
    public readonly float Step;

    public StepRangeAttribute(float min, float max, float step)
    {
        Min = min;
        Max = max;
        Step = step;
    }
}