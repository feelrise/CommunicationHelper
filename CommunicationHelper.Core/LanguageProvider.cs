using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunicationHelper.Core.Abstract;
using TranslationService;
using ITranslator = TranslationService.ITranslator;

namespace CommunicationHelper.Core
{
    public class LanguageProvider : ILocaleProvider
    {
        private readonly Dictionary<String, LanguageInfo> _allLocales = new Dictionary<String, LanguageInfo>();
        private readonly ITranslator _translator;

        private static LanguageProvider _instance;

        public static LanguageProvider GetInstance
        {
            get { return _instance ??= new LanguageProvider(); }
        }


        private LanguageProvider()
        {
            _translator = new Translator();
        }

        public LanguageInfo GetLanguageByName(String language)
        {
            return _allLocales[language];
        }

        public async Task<IEnumerable<String>> GetAllLanguages()
        {
            await InitLocales();
            return _allLocales.Select(x => x.Key);
        }

        private async Task InitLocales()
        {
            var languages = await _translator.GetAvailableLanguages();
            foreach (var availableLocale in languages)
            {
                if (!_allLocales.ContainsKey(availableLocale.Name))
                {
                    _allLocales[availableLocale.Name] = availableLocale;
                }
            }
        }

    }
}