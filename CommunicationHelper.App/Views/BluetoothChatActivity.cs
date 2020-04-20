using System;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BluetoothService;
using CommunicationHelper.Core;
using CommunicationHelper.Core.Abstract;

namespace CommunicationHelper.App.Views
{
    [Activity(Label = "Bluetooth chat")]
    public class BluetoothChatActivity : AppCompatActivity
    {
        private readonly DiscoverableModeReceiver _receiver;
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothService.BluetoothService _service;
        private Boolean _requestingPermissionsSecure, _requestingPermissionsInsecure;
        private BluetoothMessageHandler _handler;
        private WriteListener _writeListener;
        private ChatFragment _chatFrag;
        private readonly ISharedPreferencesManager _shared;

        public BluetoothChatActivity()
        {
            var context = Application.Context;
            _shared = SharedPreferencesManager.GetInstance(context);

            _receiver = new DiscoverableModeReceiver();
        }

        public void SendMessage(MessageEventArgs message)
        {
            if (_service.GetState() != Constants.STATE_CONNECTED)
            {
                Toast.MakeText(this, Resource.String.not_connected, ToastLength.Long).Show();
                return;
            }

            if (message.Message.Length > 0)
            {
                var bytes = Encoding.ASCII.GetBytes(message.Message);
                _service.Write(bytes);
            }
        }

        public override void OnRequestPermissionsResult(Int32 requestCode, String[] permissions,
                                                        Permission[] grantResults)
        {
            if (requestCode == PermissionUtils.RC_LOCATION_PERMISSIONS)
            {
                if (_requestingPermissionsSecure)
                {
                    PairWithBlueToothDevice(true);
                }
                if (_requestingPermissionsInsecure)
                {
                    PairWithBlueToothDevice(false);
                }

                _requestingPermissionsSecure = false;
                _requestingPermissionsInsecure = false;
            }
        }

        public override Boolean OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.bluetooth_menu, menu);
            return true;
        }

        public override Boolean OnPrepareOptionsMenu(IMenu menu)
        {
            var menuItem = menu.FindItem(Resource.Id.discoverable);
            menuItem?.SetEnabled(_bluetoothAdapter.ScanMode == ScanMode.ConnectableDiscoverable);

            return true;
        }

        public override Boolean OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.secure_connect_scan:
                    PairWithBlueToothDevice(true);
                    return true;
                case Resource.Id.insecure_connect_scan:
                    PairWithBlueToothDevice(false);
                    return true;
                case Resource.Id.discoverable:
                    EnsureDiscoverable();
                    return true;
            }

            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            SetContentView(Resource.Layout.content_main);

            _writeListener = new WriteListener(this);

            if (_bluetoothAdapter == null)
            {
                Toast.MakeText(this, "Bluetooth is not available.", ToastLength.Long).Show();
                FinishAndRemoveTask();
            }

            _receiver.BluetoothDiscoveryModeChanged += (sender, e) => { InvalidateOptionsMenu(); };

            InitializeFragments();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (!_bluetoothAdapter.IsEnabled)
            {
                var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableIntent, Constants.REQUEST_ENABLE_BT);
            }

            // Register for when the scan mode changes
            var filter = new IntentFilter(BluetoothAdapter.ActionScanModeChanged);
            RegisterReceiver(_receiver, filter);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_service == null)
            {
                return;
            }

            if (_service.GetState() == Constants.STATE_NONE)
            {
                _service.Start();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(_receiver);
            _service?.Stop();
        }

        protected override void OnActivityResult(Int32 requestCode, Result result, Intent data)
        {
            base.OnActivityResult(requestCode, result, data);

            switch (requestCode)
            {
                case Constants.REQUEST_CONNECT_DEVICE_SECURE:
                    if (Result.Ok == result)
                    {
                        ConnectDevice(data, true);
                    }
                    break;
                case Constants.REQUEST_CONNECT_DEVICE_INSECURE:
                    if (Result.Ok == result)
                    {
                        ConnectDevice(data, true);
                    }
                    break;
                case Constants.REQUEST_ENABLE_BT:
                    if (Result.Ok == result)
                    {
                        Toast.MakeText(this, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
                        FinishAndRemoveTask();
                    }
                    break;
            }
        }

        private void OnSendHandler(Object sender, MessageEventArgs message)
        {
            SendMessage(message);
        }

        private void ConnectDevice(Intent data, Boolean secure)
        {
            var address = data.Extras.GetString(Constants.EXTRA_DEVICE_ADDRESS);
            var device = _bluetoothAdapter.GetRemoteDevice(address);
            _service.Connect(device, secure);
            _chatFrag.OnSend -= OnSendHandler;
        }

        private void InitializeFragments()
        {
            _chatFrag = new ChatFragment(_writeListener, _shared);

            var languageFrag = new LanguageSelectorFragment(_shared);
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.language_selector_container, languageFrag)
                .Add(Resource.Id.sample_content_fragment, _chatFrag)
                .Commit();

            _handler = new BluetoothMessageHandler();
            _handler.OnHandled += OnHandled;
            _service = new BluetoothService.BluetoothService(_handler);

            _chatFrag.OnSend += OnSendHandler;
        }

        public void OnHandled(object sender, HandlerResultEventArgs args)
        {
            if (args.HandlerResult.IsStatusUpdated())
            {
                _chatFrag.SetStatus(args.Status);
            }

            if (args.HandlerResult.IsMessageAppeared())
            {
                _chatFrag.ConversationArrayAdapter.Add(args.Message);
            }

            if (args.HandlerResult.IsAlertRaised())
            {
                Toast.MakeText(_chatFrag.Activity, args.Alert,
                    ToastLength.Short).Show();
            }

            if (args.HandlerResult.ClearChat())
            {
                _chatFrag.ConversationArrayAdapter.Clear();
            }
        }

        private void PairWithBlueToothDevice(Boolean secure)
        {
            _requestingPermissionsSecure = false;
            _requestingPermissionsInsecure = false;

            if (!this.HasLocationPermissions())
            {
                _requestingPermissionsSecure = secure;
                _requestingPermissionsInsecure = !secure;
                this.RequestPermissionsForApp();
                return;
            }

            var intent = new Intent(this, typeof(DeviceListActivity));
            StartActivityForResult(intent,
                secure ? Constants.REQUEST_CONNECT_DEVICE_SECURE : Constants.REQUEST_CONNECT_DEVICE_INSECURE);
        }

        private void EnsureDiscoverable()
        {
            if (_bluetoothAdapter.ScanMode == ScanMode.ConnectableDiscoverable)
            {
                return;
            }
            var discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
            discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
            StartActivity(discoverableIntent);
        }
    }
}