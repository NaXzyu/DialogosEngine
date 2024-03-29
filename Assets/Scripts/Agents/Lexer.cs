using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DialogosEngine
{
    public static class Lexer
    {
        public static float[] Transform(string line)
        {
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

        private static bool Process(ref int @byte, ref int @float, byte[] bytes, float[] vector)
        {
            if (@byte >= bytes.Length) return false;

            int _value = 0;
            while (@byte < bytes.Length && @float < vector.Length * 4)
            {
                _value |= bytes[@byte++] << ((@byte % 4) * 8);
                if ((@byte % 4) == 0 || @byte == bytes.Length)
                {
                    unsafe
                    {
                        float* _ptr = (float*)&_value;
                        vector[@float++] = *_ptr;
                    }
                    _value = 0;
                }
            }
            return @byte < bytes.Length;
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
