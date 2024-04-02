namespace DialogosEngine
{
    public struct DictionaryEntry
    {
        public int Id { get; }
        public string Word { get; }
        public int Frequency { get; set; }

        public DictionaryEntry(int id, string word, int frequency)
        {
            Id = id;
            Word = word;
            Frequency = frequency;
        }
    }

}