using System;
using System.Collections.Generic;

namespace CommunicationHelper.Core.Abstract
{
    public interface ILanguageProvider
    {
        IEnumerable<String> GetAllLanguages();
    }
}