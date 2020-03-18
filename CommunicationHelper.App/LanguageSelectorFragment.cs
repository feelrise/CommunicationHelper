using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using CommunicationHelper.Core;
using Fragment = Android.Support.V4.App.Fragment;

namespace CommunicationHelper.App
{
    public class LanguageSelectorFragment : Fragment
    {
        private LocaleProvider _localeProvider;
        private SharedPreferencesManager _preferencesManager;
        private Spinner _spinner;

        public LanguageSelectorFragment()
        {
        }   

        private void InitializeSpinner(Spinner spinner)
        {
            var languages = _localeProvider.GetAllLanguages().ToArray();
            var adapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleSpinnerItem, languages);
            spinner.Adapter = adapter;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            InitializeSpinner(_spinner);
            _spinner.ItemSelected += (sender, args) =>
                _preferencesManager.PutValue("selected_culture", (String)_spinner.SelectedItem);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _localeProvider = new LocaleProvider();
            var context = Application.Context;
            _preferencesManager = new SharedPreferencesManager(context);

            var linearLayout = new LinearLayout(Activity) { Orientation = Orientation.Vertical };
            var textView = new TextView(Activity) { TextSize = 24, Text = "Choose the language" };
            textView.SetPadding(0, 10, 0, 0);
            linearLayout.AddView(textView);
            _spinner = new Spinner(Activity) { ScrollBarSize = 25 };
            _spinner.SetPadding(0, 10, 0, 0);
            linearLayout.AddView(_spinner);

            return linearLayout;
        }
    }
}