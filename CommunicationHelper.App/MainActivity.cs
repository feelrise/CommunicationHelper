using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Locale = Java.Util.Locale;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace CommunicationHelper.App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private FloatingActionButton _recordButton;
        private Locale _selectedLocale;
        private TextView _textContainer;
        private readonly SharedPreferencesManager _preferencesManager;
        private readonly LocaleProvider _localeProvider;
        private readonly SpeechRecognizer _speechRecognizer;
        private readonly Intent _recognitionIntent;

        public MainActivity()
        {
            var context = Application.Context;
            _preferencesManager = new SharedPreferencesManager(context);
            _localeProvider = new LocaleProvider();
            _recognitionIntent = new Intent();
            _speechRecognizer = new SpeechRecognizer(_recognitionIntent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _textContainer = FindViewById<TextView>(Resource.Id.textView1);

            InitializeFragments();
            InitializeRecordButton();
        }

        private void InitializeFragments()
        {
            var languageFrag = new LanguageSelectorFragment();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.language_selector_container, languageFrag).Commit();
        }

        private void InitializeRecordButton()
        {
            _recordButton = FindViewById<FloatingActionButton>(Resource.Id.fab);
            _recordButton.Click += FabOnClick;

            if (MicrophoneIsPresent())
            {
                _recordButton.Click += SpeechRecognition;
            }
            else
            {
                AlertNoMicrophone();
            }
        }

        private void AlertNoMicrophone()
        {
            var alert = new AlertDialog.Builder(_recordButton.Context);
            alert.SetTitle("You don't seem to have a microphone to record with");
            alert.SetPositiveButton("OK", (sender, e) => { _textContainer.Text = "No microphone present"; });
            alert.Show();
        }

        private static Boolean MicrophoneIsPresent()
        {
            const String rec = PackageManager.FeatureMicrophone;
            return rec == "android.hardware.microphone";
        }

        private void SpeechRecognition(Object sender, EventArgs args)
        {
            _selectedLocale = _localeProvider.GetLocaleByLanguage(_preferencesManager.GetValue<String>("selected_culture"));
            _speechRecognizer.SetUpVoiceIntent(_selectedLocale);
            StartActivityForResult(_recognitionIntent, 10);
        }

        public override Boolean OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override Boolean OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            return id == Resource.Id.action_settings || base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(Object sender, EventArgs eventArgs)
        {
            var view = (View)sender;
            Snackbar.Make(view, "Recognizing...", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(Int32 requestCode, String[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(Int32 requestCode, Result resultVal, Intent data)
        {
            if (requestCode == 10 && resultVal == Result.Ok)
            {
                _textContainer.Text = _speechRecognizer.GetSpeechActivityResult(data);
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }
    }
}