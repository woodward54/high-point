using System;

public static class MathUtils
{
    public static float RoundToMultipleOf(float value, float step)
    {
        // Get the absolute values of our arguments
        var absValue = Math.Abs(value);
        step = Math.Abs(step);

        // Determine the numbers on either side of value
        var low = absValue - absValue % step;
        var high = low + step;

        // Return the closest one, multiplied by -1 if value < 0
        var result = absValue - low < high - absValue ? low : high;
        return result * Math.Sign(value);
    }
}