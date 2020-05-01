using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TranslationService;
using Xamarin.Essentials;

namespace CommunicationHelper.App
{
    public class Speaker
    {
        private static Boolean _isRunning;

        private static readonly ConcurrentQueue<String> Queue = new ConcurrentQueue<String>();

        public static async Task SpeakAsync(String message, LanguageInfo locale)
        {
            Queue.Enqueue(message);

            if (!_isRunning)
            {
                _isRunning = true;
                await Run(locale);  
            }
        }

        private static async Task Run(LanguageInfo locale)
        {
            while (!Queue.IsEmpty)
            {
                var dequeueSuccess = Queue.TryDequeue(out var message);
                if (dequeueSuccess)
                {
                    await Speak(message, locale);
                }
            }
            _isRunning = false;
        }

        private static async Task Speak(String message, LanguageInfo locale)
        {
            var locales = await TextToSpeech.GetLocalesAsync();

            var settings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = 1.0f,
                Locale = locales.FirstOrDefault(item => item.Language == locale.Key)
            };

            await TextToSpeech.SpeakAsync(message, settings);
        }
    }
}