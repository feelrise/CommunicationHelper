using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunicationHelper.Core.Abstract
{
    public interface ILanguageProvider
    {
        Task<IEnumerable<String>> GetAllLanguages();
    }
}