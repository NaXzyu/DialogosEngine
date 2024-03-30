using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogosEngine
{
    public static class Lexer
    {
        public const int k_MaxChars = 1000;
        private static bool _isLittleEndian;
        public static bool IsLittleEndian => _isLittleEndian;

        public static float[] Vectorize(string line)
        {

            if (line != null && line.Length > k_MaxChars)
            {
                throw new LexerException($"Input exceeds the maximum length of {k_MaxChars} characters.");
            }

            try
            {
                byte[] _bytes = System.Text.Encoding.ASCII.GetBytes(line);
                int _size = (int)Math.Ceiling(_bytes.Length / 4.0);
                float[] _vector = new float[_size];
                int _float = 0;
                int _byte = 0;

                while (Process(ref _byte, ref _float, _bytes, _vector)) ;
                return _vector;
            }
            catch (Exception ex)
            {
                throw new LexerException("An error occurred while processing", ex);
            }
        }


        public static bool Process(ref int byteIndex, ref int floatIndex, byte[] bytes, float[] vector)
        {
            if (byteIndex >= bytes.Length) return false;

            return ProcessBytesToFloats(ref byteIndex, ref floatIndex, bytes, vector);
        }

        private static bool ProcessBytesToFloats(ref int byteIndex, ref int floatIndex, byte[] bytes, float[] vector)
        {
            while (byteIndex < bytes.Length && floatIndex < vector.Length)
            {
                vector[floatIndex++] = ConvertBytesToSingle(ref byteIndex, bytes);
            }
            return byteIndex < bytes.Length;
        }

        private static byte[] GetNextBytes(ref int byteIndex, byte[] bytes)
        {
            byte[] _bytes = new byte[4];
            int _bytesToCopy = Math.Min(4, bytes.Length - byteIndex);
            Array.Copy(bytes, byteIndex, _bytes, 0, _bytesToCopy);
            byteIndex += _bytesToCopy;
            return _bytes;
        }

        private static float ConvertBytesToSingle(ref int byteIndex, byte[] bytes)
        {
            byte[] _bytes = GetNextBytes(ref byteIndex, bytes);
            if (!IsLittleEndian)
            {
                Array.Reverse(_bytes);
            }
            return BitConverter.ToSingle(_bytes, 0);
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
