using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DialogosEngine
{
    public static class Lexer
    {
        public const int k_MaxChars = 1000;
        public const int k_MaxBufferLength = 1000;
        public const float k_ByteMultiplier = 1.0f / (1 << 23);

        public static float[] VectorizeUTF8(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line), "Input string cannot be null.");
            }

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(line);
            if (utf8Bytes.Length > k_MaxBufferLength)
            {
                throw new LexerException($"Vectorized buffer exceeds the maximum length of {k_MaxBufferLength} floats.");
            }

            float[] vector = new float[utf8Bytes.Length];
            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                vector[i] = utf8Bytes[i] * k_ByteMultiplier;
            }

            return vector;
        }

        public static string QuantizeUTF8(float[] vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException(nameof(vector), "Input vector cannot be null.");
            }

            byte[] utf8Bytes = new byte[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                utf8Bytes[i] = (byte)(vector[i] / k_ByteMultiplier);
            }

            Decoder utf8Decoder = Encoding.UTF8.GetDecoder();
            int charCount = utf8Decoder.GetCharCount(utf8Bytes, 0, utf8Bytes.Length);
            if (charCount > k_MaxBufferLength)
            {
                throw new LexerException($"Output exceeds the maximum length of {k_MaxBufferLength} characters.");
            }

            char[] chars = new char[charCount];
            utf8Decoder.GetChars(utf8Bytes, 0, utf8Bytes.Length, chars, 0);

            return new string(chars);
        }

        public static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                return string.IsNullOrEmpty(b) ? 0 : new StringInfo(b).LengthInTextElements;
            }

            if (string.IsNullOrEmpty(b))
            {
                return new StringInfo(a).LengthInTextElements;
            }

            TextElementEnumerator enumeratorA = StringInfo.GetTextElementEnumerator(a);
            TextElementEnumerator enumeratorB = StringInfo.GetTextElementEnumerator(b);

            List<string> textElementsA = new List<string>();
            while (enumeratorA.MoveNext())
            {
                textElementsA.Add((string)enumeratorA.Current);
            }

            List<string> textElementsB = new List<string>();
            while (enumeratorB.MoveNext())
            {
                textElementsB.Add((string)enumeratorB.Current);
            }

            int lengthA = textElementsA.Count;
            int lengthB = textElementsB.Count;
            int[] previousRow = new int[lengthB + 1];
            int[] currentRow = new int[lengthB + 1];

            for (int j = 0; j <= lengthB; j++)
            {
                previousRow[j] = j;
            }

            for (int i = 1; i <= lengthA; i++)
            {
                currentRow[0] = i;

                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = textElementsB[j - 1] == textElementsA[i - 1] ? 0 : 1;
                    currentRow[j] = Math.Min(
                        Math.Min(currentRow[j - 1] + 1, previousRow[j] + 1),
                        previousRow[j - 1] + cost);
                }

                (currentRow, previousRow) = (previousRow, currentRow);
            }

            return previousRow[lengthB];
        }

        public static float CalculateWhitespace(string[] text)
        {
            int _totalWhitespace = text.Sum(line => line.Count(char.IsWhiteSpace));
            int _totalChars = text.Sum(line => line.Length);
            float _whitespaceRatio = (float)_totalWhitespace / _totalChars;
            float _sigmoidWhitespaceRatio = 1f / (1f + (float)Math.Exp(-_whitespaceRatio));

            return _sigmoidWhitespaceRatio;
        }

        public static float CalculateVariance(string[] text)
        {
            float _sumOfLengths = 0;
            foreach (string _line in text)
            {
                _sumOfLengths += _line.Length;
            }
            float _meanLength = _sumOfLengths / text.Length;
            float _sumOfSquaredDiff = 0;
            foreach (string _line in text)
            {
                _sumOfSquaredDiff += (_line.Length - _meanLength) * (_line.Length - _meanLength);
            }

            return _sumOfSquaredDiff / text.Length;
        }

        public static float CalculateDiversity(string[] buffer, int size)
        {
            var _charCounts = new Dictionary<char, int>();
            foreach (var line in buffer)
            {
                foreach (var character in line)
                {
                    if (!_charCounts.ContainsKey(character))
                    {
                        _charCounts[character] = 0;
                    }
                    _charCounts[character]++;
                }
            }
            float _entropy = 0f;
            foreach (var _count in _charCounts.Values)
            {
                float _probability = (float)_count / size;
                _entropy -= _probability * (float)Math.Log(_probability, 2);
            }
            float _maxEntropy = (float)Math.Log(_charCounts.Count, 2);
            float _normalizedEntropy = _entropy / _maxEntropy;

            return _normalizedEntropy;
        }

        public static Quaternion CreateQuaternion(string[] text, int size)
        {
            float _x = text.Length;
            float _y = CalculateVariance(text);
            float _z = CalculateWhitespace(text);
            float _w = CalculateDiversity(text, size);

            Quaternion _quaternion = new Quaternion(_x, _y, _z, _w);
            _quaternion.Normalize();

            return _quaternion;
        }
    }
}
