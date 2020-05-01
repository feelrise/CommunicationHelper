using System;
using Java.Util;
using TranslationService;

namespace CommunicationHelper.Core.Abstract
{
    public interface ILocaleProvider :  ILanguageProvider
    {
        LanguageInfo GetLanguageByName(String language);

        Locale GetLocaleByName(String language);
    }
}   