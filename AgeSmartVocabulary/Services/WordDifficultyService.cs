namespace AgeSmartVocabulary.Services
{
    public static class WordDifficultyService
    {
        public static bool IsSimpleWordForKids(string word, int frequency, int syllables)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;

            // Rule 1: Word length
            if (word.Length > 6)
                return false;

            // Rule 2: Frequency (Datamuse score)
            if (frequency < 2000)
                return false;

            // Rule 3: Syllables
            if (syllables > 2)
                return false;

            // Rule 4: Only alphabets
            if (!word.All(char.IsLetter))
                return false;

            return true;
        }
    }
}
