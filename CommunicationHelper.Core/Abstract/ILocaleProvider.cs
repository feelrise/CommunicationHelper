using System;
using Java.Util;

namespace CommunicationHelper.Core.Abstract
{
    public interface ILocaleProvider :  ILanguageProvider
    {
        Locale GetLocaleByLanguage(String language);
    }
}