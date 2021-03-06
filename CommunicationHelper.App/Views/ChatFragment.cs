﻿using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CommunicationHelper.Core.Abstract;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Fragment = Android.Support.V7.App.AppCompatDialogFragment;

namespace CommunicationHelper.App.Views
{
    public class ChatFragment : Fragment
    {
        private readonly TextView.IOnEditorActionListener _listener;
        private readonly ISharedPreferencesManager _preferencesManager;
        private readonly ILocaleProvider _localeProvider;
        private readonly Intent _recognitionIntent;
        private readonly SpeechRecognizer _speechRecognizer;
        private ListView _conversationView;
        private EditText _outEditText;
        private Button _sendButton;
        private ImageButton _recordButton;
        private String _selectedLocale;

        public ChatFragment(TextView.IOnEditorActionListener listener, ISharedPreferencesManager preferences,
                            ILocaleProvider localeProvider)
        {
            _listener = listener;

            _preferencesManager = preferences;
            _localeProvider = localeProvider;
            _recognitionIntent = new Intent();
            _speechRecognizer = new SpeechRecognizer(_recognitionIntent);
        }

        public ArrayAdapter<BluetoothMessage> ConversationArrayAdapter { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.bluetooth_chat_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            _conversationView = view.FindViewById<ListView>(Resource.Id.@in);
            _outEditText = view.FindViewById<EditText>(Resource.Id.edit_text_out);
            _sendButton = view.FindViewById<Button>(Resource.Id.button_send);
            _recordButton = view.FindViewById<ImageButton>(Resource.Id.button_record);

            SetupChat();
        }

        public event EventHandler<MessageEventArgs> OnSend;

        public event EventHandler<String> OnMessageClick;

        public void SetStatus(Int32 resId)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.SetSubtitle(resId);
            }
        }

        public void SetStatus(String subTitle)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.Subtitle = subTitle;
            }
        }

        public override void OnActivityResult(Int32 requestCode, Int32 resultCode, Intent data)
        {
            if (requestCode == 10 && resultCode == (Int32) Result.Ok)
            {
                OnSendInvoke(new MessageEventArgs {Message = _speechRecognizer.GetSpeechActivityResult(data)});
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected virtual void OnSendInvoke(MessageEventArgs e)
        {
            OnSend?.Invoke(this, e);
        }

        protected virtual void RaiseOnMessageClick(String e)
        {
            OnMessageClick?.Invoke(this, e);
        }


        private void SetupChat()
        {
            ConversationArrayAdapter = new ArrayAdapter<BluetoothMessage>(Activity, Resource.Layout.message);
            _conversationView.Adapter = ConversationArrayAdapter;

            _conversationView.ItemClick += (sender, e) =>
            {
                RaiseOnMessageClick(((BluetoothMessage) _conversationView.GetItemAtPosition(e.Position)).Message);
            };

            _outEditText.SetOnEditorActionListener(_listener);
            _sendButton.Click += (sender, e) =>
            {
                var textView = View.FindViewById<TextView>(Resource.Id.edit_text_out);
                var msg = textView.Text;
                textView.Text = string.Empty;
                OnSendInvoke(new MessageEventArgs {Message = msg});
            };
            InitializeRecordButton();
        }

        private void InitializeRecordButton()
        {
            if (MicrophoneIsPresent())
            {
                _recordButton.Click += StartSpeechRecognition;
            }
            else
            {
                AlertNoMicrophone();
            }
        }

        private void StartSpeechRecognition(Object sender, EventArgs args)
        {
            _selectedLocale = _preferencesManager.GetValue<String>("selected_culture");

            _speechRecognizer.SetUpVoiceIntent(_localeProvider.GetLocaleByName(_selectedLocale));

            StartActivityForResult(_recognitionIntent, 10);
        }

        private void AlertNoMicrophone()
        {
            var alert = new AlertDialog.Builder(_recordButton.Context);
            alert.SetTitle("You don't seem to have a microphone to record with");
            alert.SetPositiveButton("OK",
                (sender, e) => { Toast.MakeText(Activity, "No microphone present", ToastLength.Short); });
            alert.Show();
        }

        private static Boolean MicrophoneIsPresent()
        {
            const String rec = PackageManager.FeatureMicrophone;
            return rec == "android.hardware.microphone";
        }

        private Boolean HasActionBar()
        {
            return Activity?.ActionBar != null;
        }
    }
}