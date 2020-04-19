using System;
using TranslationService;

namespace CommunicationHelper.App
{
    public class MessageEventArgs : EventArgs
    {
        public String Message { get; set; }

        public LanguageInfo InitialLanguage { get; set; }
    }   
}