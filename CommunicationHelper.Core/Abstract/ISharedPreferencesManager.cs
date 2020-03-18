using System;

namespace CommunicationHelper.Core.Abstract
{
    public interface ISharedPreferencesManager
    {
        void PutValue<T>(String key, T input);

        T GetValue<T>(String key);
    }
}