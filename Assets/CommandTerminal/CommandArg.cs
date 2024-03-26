namespace CommandTerminal
{
    public struct CommandArg
    {
        public string String { get; set; }

        public int Int
        {
            get
            {
                int int_value;

                if (int.TryParse(String, out int_value))
                {
                    return int_value;
                }

                return 0;
            }
        }

        public float Float
        {
            get
            {
                float float_value;

                if (float.TryParse(String, out float_value))
                {
                    return float_value;
                }

                return 0;
            }
        }

        public bool Bool
        {
            get
            {
                if (string.Compare(String, "TRUE", ignoreCase: true) == 0)
                {
                    return true;
                }

                if (string.Compare(String, "FALSE", ignoreCase: true) == 0)
                {
                    return false;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return String;
        }
    }
}