using System;
using Android.Content;
using Android.Preferences;
using Java.Lang;
using Boolean = System.Boolean;
using Object = System.Object;
using String = System.String;
using Float = System.Single;
namespace CommunicationHelper.App
{
    public class SharedPreferencesManager
    {
        private readonly ISharedPreferences _sharedPreferences;
        private readonly ISharedPreferencesEditor _preferencesEditor;

        public SharedPreferencesManager(Context context)
        {
            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
            _preferencesEditor = _sharedPreferences.Edit();
        }

        public void PutValue<T>(String key, T input)
        {
            lock (_lock)
            {
                switch (input)
                {
                    case String value:
                        _preferencesEditor.PutString(key, value);
                        break;
                    case Int32 value:
                        _preferencesEditor.PutInt(key, value);
                        break;
                    case Boolean value:
                        _preferencesEditor.PutBoolean(key, value);
                        break;
                    case Float value:
                        _preferencesEditor.PutFloat(key, value);
                        break;
                    default:
                        throw new UnsupportedOperationException($"Method {nameof(PutValue)} does not support {typeof(T)}");
                }

                _preferencesEditor.Apply();
            }
        }

        public T GetValue<T>(String key)
        {
            Object result;
            switch (typeof(T).Name)
            {
                case "String":
                    result = _sharedPreferences.GetString(key, default);
                    break;
                case "Int32":
                    result = _sharedPreferences.GetInt(key, default);
                    break;
                case "Boolean":
                    result = _sharedPreferences.GetBoolean(key, default);
                    break;
                case "Single":
                    result = _sharedPreferences.GetFloat(key, default);
                    break;
                default:
                    return default;
            }

            return (T) Convert.ChangeType(result, typeof(T));
        }

        private readonly Object _lock = new Object();
    }
}