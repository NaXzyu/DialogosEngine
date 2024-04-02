using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DialogosEngine
{
    public class DictionaryManager
    {
        public Dictionary<string, (int id, int frequency)> WordDictionary = new();
        private int _NextAvailableId = 0;
        private const int k_MaxDictionarySize = 10000000;
        private const string k_DictionaryName = "dictionary";
        private const string k_DictionaryExt = "bin";
        private const string k_DateSuffixFormat = "yyMMdd";

        private int FrequencyThreshold => 5;

        public void UpdateFrequency(string word)
        {
            if (WordDictionary.ContainsKey(word))
            {
                var entry = WordDictionary[word];
                entry.frequency++;
                WordDictionary[word] = entry;
            }
            else if (WordDictionary.Count < k_MaxDictionarySize)
            {
                WordDictionary.Add(word, (_NextAvailableId++, 1));
            }
        }

        public void PruneDictionary()
        {
            var infrequentWords = WordDictionary
                .Where(pair => pair.Value.frequency < FrequencyThreshold)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var word in infrequentWords)
            {
                WordDictionary.Remove(word);
            }
        }
        public void SaveDictionaryToBinaryFile(string directoryPath)
        {
            string fileName = k_DictionaryName + "." + k_DictionaryExt;
            string filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                string dateSuffix = DateTime.Now.ToString(k_DateSuffixFormat);
                string newFileName = $"{k_DictionaryName}.{dateSuffix}.{k_DictionaryExt}";
                string newFilePath = Path.Combine(directoryPath, newFileName);
                File.Move(filePath, newFilePath);
            }

            BinaryFormatter formatter = new();
            using FileStream stream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, WordDictionary);
        }

        public void LoadDictionaryFromBinaryFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                WordDictionary = (Dictionary<string, (int id, int frequency)>)formatter.Deserialize(stream);
            }
            else
            {
                Debug.LogError("Dictionary file not found.");
            }
        }
    }
}