namespace AgeSmartVocabulary.Services
{
    public class ContentFilterService
    {
        // Words to block (inappropriate content)
        private static readonly HashSet<string> BlockedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Explicit terms
            "sex", "sexual", "sexuality", "penis", "vagina", "breast", "orgasm", "erotic",
            "porn", "pornography", "nude", "naked", "intercourse", "prostitute",
            
            // Violence/weapons
            "kill", "murder", "weapon", "gun", "knife", "blood", "death", "suicide",
            "torture", "violent", "rape", "assault",
            
            // Drugs/alcohol
            "drug", "cocaine", "heroin", "marijuana", "alcohol", "drunk", "beer", "wine",
            "cigarette", "smoking", "tobacco",
            
            // Profanity (add as needed)
            "damn", "hell", "crap", "shit", "fuck", "ass", "bitch",
            
            // Body parts that might have inappropriate definitions
            "butt", "boob", "genitals", "anus", "rectum",
            
            // Other sensitive topics
            "war", "bomb", "terrorist", "nazi", "slave", "racism"
        };

        // Words that might have double meanings - check definition carefully
        private static readonly HashSet<string> CautionWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "cock", "pussy", "ass", "dick", "balls", "screw", "shaft", "blow"
        };

        /// <summary>
        /// Check if word is safe for children
        /// </summary>
        public bool IsSafeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;

            var cleanWord = word.Trim().ToLower();

            // Block if in blocked list
            if (BlockedWords.Contains(cleanWord))
            {
                System.Diagnostics.Debug.WriteLine($"❌ Blocked word: {word}");
                return false;
            }

            // Block if contains blocked substring
            foreach (var blocked in BlockedWords)
            {
                if (cleanWord.Contains(blocked))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Blocked (contains): {word}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if definition contains inappropriate content
        /// </summary>
        public bool IsSafeDefinition(string definition)
        {
            if (string.IsNullOrWhiteSpace(definition))
                return false;

            var lowerDef = definition.ToLower();

            // Check for blocked words in definition
            foreach (var blocked in BlockedWords)
            {
                if (lowerDef.Contains(blocked))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Blocked definition containing: {blocked}");
                    return false;
                }
            }

            // Check for sensitive phrases
            var sensitivePatterns = new[]
            {
                "sexual", "genitals", "reproductive", "intimate", "explicit",
                "violence", "death", "killing", "suicide"
            };

            foreach (var pattern in sensitivePatterns)
            {
                if (lowerDef.Contains(pattern))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Blocked definition with pattern: {pattern}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if example is safe
        /// </summary>
        public bool IsSafeExample(string example)
        {
            if (string.IsNullOrWhiteSpace(example))
                return true; // Empty example is fine

            return IsSafeDefinition(example); // Use same logic as definition
        }
    }
}