using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DialogosEngine
{
    public static class Lexer
    {
        public const int k_MaxChars = 1000;
        public const int k_CharsPerFloat = 2;
        private static bool _isLittleEndian;
        public static bool IsLittleEndian => _isLittleEndian;

        public static float[] powersOfTen = Enumerable.Range(0, k_CharsPerFloat).Select(x => (float)Math.Pow(10, 3 * x)).ToArray();

        public static float[] Vectorize(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line), "Input string cannot be null.");
            }
            if (line.Length > k_MaxChars)
            {
                var message = new StringBuilder("Input exceeds the maximum length of ")
                    .Append(k_MaxChars)
                    .Append(" characters.")
                    .ToString();
                throw new LexerException(message);
            }

            int vectorSize = (int)Math.Ceiling((float)line.Length / k_CharsPerFloat);
            float[] vector = new float[vectorSize];
            const float multiplier = 0.000001f;

            for (int i = 0, j = 0; i < line.Length; i += k_CharsPerFloat, j++)
            {
                float packedValue = 0;
                int charsToProcess = Math.Min(k_CharsPerFloat, line.Length - i);

                for (int k = 0; k < charsToProcess; k++)
                {
                    // Inline ASCII conversion
                    packedValue += (line[i + k] * powersOfTen[charsToProcess - k - 1]);
                }

                vector[j] = packedValue * multiplier;
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
