using UnityEngine;

public static class MathUtility
{
    public static int WeightRandom(int[] weights)
    {
        int weightSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            weightSum += weights[i];
        }
        float randomValue = Random.value * weightSum;

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
            {
                return i;
            }
            randomValue -= weights[i];
        }
        return weights.Length - 1;
    }
    
    /// <summary>
    /// unit: degree
    /// </summary>
    public static float GetHorizontalFOV(float verticalFOV, float viewportWidth, float viewportHeight)
    {
        return Mathf.Atan(Mathf.Tan(verticalFOV * Mathf.Deg2Rad * 0.5f) * viewportWidth / viewportHeight) * Mathf.Rad2Deg * 2;
    }
    
    /// <summary>
    /// unit: degree
    /// </summary>
    public static float GetVerticalFOV(float horizontalFOV, float viewportWidth, float viewportHeight)
    {
        return Mathf.Atan(Mathf.Tan(horizontalFOV * Mathf.Deg2Rad * 0.5f) * viewportHeight / viewportWidth) * Mathf.Rad2Deg * 2;
    }
}
