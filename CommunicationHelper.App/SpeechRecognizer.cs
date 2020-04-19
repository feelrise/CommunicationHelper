using System;
using System.Linq;
using Android.Content;
using Android.Speech;
using Locale = Java.Util.Locale;

namespace CommunicationHelper.App
{
    public class SpeechRecognizer
    {
        private readonly Intent _intent;

        public SpeechRecognizer(Intent intent)
        {
            _intent = intent;
        }

        public void SetUpVoiceIntent(Locale selectedLocale)
        {
            _intent.SetAction(RecognizerIntent.ActionRecognizeSpeech);
            // put a message on the modal dialog
            _intent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");
            // if there is more then 1.5s of silence, consider the speech over
            _intent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 15000);
            _intent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 15000);
            _intent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            _intent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            _intent.PutExtra(RecognizerIntent.ExtraLanguageModel, selectedLocale.Language);
            _intent.PutExtra(RecognizerIntent.ExtraLanguage, selectedLocale.Language);
            _intent.PutExtra(RecognizerIntent.ExtraLanguagePreference, selectedLocale.Language);
            _intent.PutExtra(RecognizerIntent.ExtraOnlyReturnLanguagePreference, selectedLocale.Language);
        }   
        public String GetSpeechActivityResult(Intent data)
        {
            var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
            if (!matches.Any()) 
                return "No speech was recognized";

            var textInput = matches[0];
            if (textInput.Length > 500)
                textInput = textInput.Substring(0, 500);
            return textInput;
        }
    }
}