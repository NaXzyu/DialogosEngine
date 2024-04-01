using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DialogosEngine
{
    public static class Lexer
    {
        public const int k_MaxChars = 1000;
        private static bool _isLittleEndian;
        public static bool IsLittleEndian => _isLittleEndian;

        public static float[] Vectorize(string line, int charsPerFloat = 2)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line), "Input string cannot be null.");
            }
            if (line.Length > k_MaxChars)
            {
                throw new LexerException($"Input exceeds the maximum length of {k_MaxChars} characters.");
            }

            // Pre-calculate powers of 10
            double[] powersOfTen = Enumerable.Range(0, charsPerFloat).Select(x => Math.Pow(10, 3 * x)).ToArray();

            // Calculate the size of the vector
            int vectorSize = (int)Math.Ceiling((double)line.Length / charsPerFloat);
            float[] vector = new float[vectorSize];

            // Define a multiplier based on the precision of a float
            const float multiplier = 0.000001f; // Adjusted for six digits of precision

            for (int i = 0, j = 0; i < line.Length; i += charsPerFloat, j++)
            {
                double packedValue = 0;

                for (int k = 0; k < charsPerFloat && (i + k) < line.Length; k++)
                {
                    char character = line[i + k];
                    int asciiValue = character;
                    // Use pre-calculated power of 10
                    packedValue += asciiValue * powersOfTen[charsPerFloat - k - 1];
                }

                // Multiply by the fixed multiplier to shift the decimal point
                vector[j] = (float)(packedValue * multiplier);
            }

            return vector;
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

        public static bool CheckEndianness()
        {
            ushort _testNumber = 0x1234;
            byte[] _testBytes = BitConverter.GetBytes(_testNumber);
            return _testBytes[0] == 0x34;
        }

        public static void Reset()
        {
            _isLittleEndian = CheckEndianness();
        }
    }
}
