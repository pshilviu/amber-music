using Amber.Music.Domain.Services;

namespace Amber.Music.Services
{
    public class WordCounterService : IWordCounterService
    {
        public int Count(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
            {
                index++;
            }

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                {
                    index++;
                }

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                {
                    index++;
                }
            }

            return wordCount;
        }
    }
}
