using System;

namespace TranslationService
{
    public class Translation
    {
        public String Text { get; set; }

        public TextResult Transliteration { get; set; }

        public String To { get; set; }

        public Alignment Alignment { get; set; }

        public SentenceLength SentLen { get; set; }
    }
}