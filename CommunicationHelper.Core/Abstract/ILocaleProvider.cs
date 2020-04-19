using System;
using TranslationService;

namespace CommunicationHelper.Core.Abstract
{
    public interface ILocaleProvider :  ILanguageProvider
    {
        LanguageInfo GetLanguageByName(String language);
    }
}   