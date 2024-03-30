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

        public static float[] VectorizeNew(string line, int charsPerFloat = 3)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line), "Input string cannot be null.");
            }

            if (line.Length > k_MaxChars)
            {
                throw new LexerException($"Input exceeds the maximum length of {k_MaxChars} characters.");
            }

            // Define a path for the log file on the desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFilePath = Path.Combine(desktopPath, "VectorizeNew.log");

            // Helper method to log messages to a file
            void LogToFile(string message)
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
            }

            // Calculate the size of the vector based on the number of characters per float
            int vectorSize = (int)Math.Ceiling((double)line.Length / charsPerFloat);
            float[] vector = new float[vectorSize];

            // Log the initial values
            LogToFile($"Vectorizing string: {line}");
            LogToFile($"charsPerFloat: {charsPerFloat}");
            LogToFile($"Resulting array size: {vector.Length}");

            for (int i = 0, j = 0; i < line.Length; i += charsPerFloat, j++)
            {
                int packedValue = 0;

                // Log the current state before packing
                LogToFile($"Current line index: {i}");
                LogToFile($"Current array index: {j}");

                // Pack characters into the integer
                for (int k = 0; k < charsPerFloat && (i + k) < line.Length; k++)
                {
                    LogToFile($"Packing char '{line[i + k]}' (ASCII: {(int)line[i + k]}) at bit position: {(k * 8)}");

                    packedValue |= line[i + k] << (k * 8);
                    
                    LogToFile($"Packed value: {packedValue}");
                }

                // Convert the packed integer to a float
                vector[j] = BitConverter.ToSingle(BitConverter.GetBytes(packedValue), 0);

                // Log the packed float value
                LogToFile($"Packed float at index {j}: {vector[j]}");
            }

            // Log the final float array
            LogToFile($"Final float array: {string.Join(", ", vector)}");

            return vector;
        }

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
