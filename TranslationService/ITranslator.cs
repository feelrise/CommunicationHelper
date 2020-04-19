using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TranslationService
{
    public interface ITranslator
    {
        Task<TranslateResult> Translate(String original, String initialLanguage, String targetLanguage);

        Task<TranslateResult> Translate(String original, String targetLanguage);

        Task<IEnumerable<LanguageInfo>> GetAvailableLanguages();
    }
}   