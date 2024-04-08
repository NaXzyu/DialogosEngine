namespace DialogosEngine.Tests
{
    public static class Utility
    {
        public static string FormatFloatArray(float[] floatArray)
        {
            return string.Join(", ", floatArray.Select(f => f.ToString("G7")));
        }
    }
}
