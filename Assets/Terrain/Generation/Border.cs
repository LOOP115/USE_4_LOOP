using UnityEngine;

public static class Border
{
   public static float[,] GenerateBorder(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1f;
                float y = j / (float)size * 2 - 1f;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = FadingCurve(value);
            }
        }

        return map;
    }

    static float FadingCurve(float value)
    {
        float a = 3.2f;
        float b = 2.3f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) +Mathf.Pow(b - b * value, a));
    }


}
