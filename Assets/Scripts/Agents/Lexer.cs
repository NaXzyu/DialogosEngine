using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DialogosEngine
{
    public static class Lexer
    {
        public const int k_MaxChars = 1000;
        public const int k_CharsPerFloat = 2;
        private static bool _isLittleEndian;
        public static bool IsLittleEndian => _isLittleEndian;

        public static float[] PowersOfTen = Enumerable.Range(0, k_CharsPerFloat).Select(x => (float)Math.Pow(10, 3 * x)).ToArray();
        public const float MultiplierASCII = 0.000001f;
        public const float MultiplierUTF8 = 1.0f / (1 << 23);

        public static float[] Vectorize(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line), "Input string cannot be null.");
            }
            if (line.Length > k_MaxChars)
            {
                var message = new StringBuilder(50)
                    .Append("Input exceeds the maximum length of ")
                    .Append(k_MaxChars)
                    .Append(" characters.")
                    .ToString();
                throw new LexerException(message);
            }

            int vectorSize = (int)Math.Ceiling((float)line.Length / k_CharsPerFloat);
            float[] vector = new float[vectorSize];

            for (int i = 0, j = 0; i < line.Length; i += k_CharsPerFloat, j++)
            {
                float packedValue = 0;
                int charsToProcess = Math.Min(k_CharsPerFloat, line.Length - i);

                for (int k = 0; k < charsToProcess; k++)
                {
                    // Inline ASCII conversion
                    packedValue += (line[i + k] * PowersOfTen[charsToProcess - k - 1]);
                }

                vector[j] = packedValue * MultiplierASCII;
            }

            return vector;
        }

        public static float[] VectorizeUTF8(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line), "Input string cannot be null.");
            }
            if (line.Length > k_MaxChars)
            {
                var message = new StringBuilder(50)
                    .Append("Input exceeds the maximum length of ")
                    .Append(k_MaxChars)
                    .Append(" characters.")
                    .ToString();
                throw new LexerException(message);
            }

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(line);
            float[] vector = new float[utf8Bytes.Length];

            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                vector[i] = utf8Bytes[i] * MultiplierUTF8;
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
            int index = 0;
            while (index < vector.Length)
            {
                utf8Bytes[index] = (byte)(vector[index] / MultiplierUTF8);
                index++;
            }

            Decoder utf8Decoder = Encoding.UTF8.GetDecoder();
            int charCount = utf8Decoder.GetCharCount(utf8Bytes, 0, utf8Bytes.Length);
            if (charCount > k_MaxChars)
            {
                throw new LexerException($"Output exceeds the maximum length of {k_MaxChars} characters.");
            }

            char[] chars = new char[charCount];
            utf8Decoder.GetChars(utf8Bytes, 0, utf8Bytes.Length, chars, 0);

            return new string(chars);
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
