using System;

namespace DialogosEngine
{
    public class Lexer
    {
        public float[] Transform(string line)
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
                throw new LexerException("An error occurred while processing the line.", ex);
            }
        }

        private bool Process(ref int @byte, ref int @float, byte[] bytes, float[] vector)
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
    }
}
