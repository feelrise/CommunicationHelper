using System;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using CommunicationHelper.Core;
using CommunicationHelper.Core.Abstract;
using Fragment = Android.Support.V4.App.Fragment;

namespace CommunicationHelper.App.Views
{
    public class LanguageSelectorFragment : Fragment
    {
        private readonly ILocaleProvider _localeProvider;
        private readonly ISharedPreferencesManager _preferencesManager;
        private Spinner _spinner;

        public LanguageSelectorFragment(ISharedPreferencesManager sharedPreferences)
        {
            _localeProvider = LanguageProvider.GetInstance;
            _preferencesManager = sharedPreferences;
        }

        private async Task InitializeSpinner(Spinner spinner)
        {
            var languages = await _localeProvider.GetAllLanguages();
            var adapter = new ArrayAdapter<String>(Activity, Android.Resource.Layout.SimpleSpinnerItem, languages.ToArray());
            spinner.Adapter = adapter;
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            await InitializeSpinner(_spinner);
            _spinner.ItemSelected += (sender, args) =>
                _preferencesManager.PutValue("selected_culture", (String)_spinner.SelectedItem);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var linearLayout = new LinearLayout(Activity) { Orientation = Orientation.Vertical };
            var textView = new TextView(Activity) { TextSize = 24, Text = "Choose the language" };
            
            textView.SetTextColor(Android.Graphics.Color.Black);
            textView.SetPadding(10, 10, 0, 0);
            linearLayout.AddView(textView);
            _spinner = new Spinner(Activity) { ScrollBarSize = 55 };
            _spinner.SetPadding(10, 10, 0, 0);
            linearLayout.AddView(_spinner);

            return linearLayout;
        }
    }
}