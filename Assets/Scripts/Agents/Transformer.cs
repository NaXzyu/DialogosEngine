using System.Linq;
using UnityEngine;

namespace DialogosEngine
{
    public static class Transformer
    {
        public static float Transform(ref float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);
            return (value + 1f) / 2f;
        }

        public static int RoundMax(ref float value)
        {
            return Mathf.RoundToInt(Transform(ref value) * Lexer.k_MaxBufferLength);
        }

        public static float[] SoftMax(ref float[] values)
        {
            float max = values.Max();
            float scale = values.Sum(v => Mathf.Exp(v - max));
            return values.Select(v => Mathf.Exp(v - max) / scale).ToArray();
        }

    }

}