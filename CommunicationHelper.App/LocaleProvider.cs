using System;
using System.Collections.Generic;
using System.Linq;
using Java.Util;

namespace CommunicationHelper.App
{
    public class LocaleProvider
    {
        private readonly Dictionary<String, Locale> _allLocales = new Dictionary<String, Locale>();

        public LocaleProvider()
        {
            InitLocales();
        }

        public Locale GetLocaleByLanguage(String language)
        {
            return _allLocales[language];
        }

        public IEnumerable<String> GetAllLanguages()
        {
            return _allLocales.Select(x => x.Key).ToArray();
        }

        private void InitLocales()
        {
            foreach (var availableLocale in Locale.GetAvailableLocales().OrderBy(x=>x.DisplayLanguage))
            {
                if (!_allLocales.ContainsKey(availableLocale.DisplayLanguage))
                {
                    _allLocales[availableLocale.DisplayLanguage] = availableLocale;
                }
            }
        }
    }
}