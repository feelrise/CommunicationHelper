using System;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using BluetoothService;

namespace CommunicationHelper.App.Views
{
    [Activity(Label = "DeviceListActivity")]
    public class DeviceListActivity : Activity
    {
        private static ArrayAdapter<String> _newDevicesArrayAdapter;

        private BluetoothAdapter _btAdapter;
        private DeviceDiscoveredReceiver _receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Setup the window
            RequestWindowFeature(WindowFeatures.IndeterminateProgress);
            SetContentView(Resource.Layout.bluetooth_device_list_activity);

            // Set result CANCELED incase the user backs out
            SetResult(Result.Canceled);

            // Initialize the button to perform device discovery			
            var scanButton = FindViewById<Button>(Resource.Id.button_scan);
            scanButton.Click += (sender, e) =>
            {
                DoDiscovery();
                ((View) sender).Visibility = ViewStates.Gone;
            };

            // Initialize array adapters. One for already paired devices and
            // one for newly discovered devices
            var pairedDevicesArrayAdapter = new ArrayAdapter<String>(this, Resource.Layout.device_name);
            _newDevicesArrayAdapter = new ArrayAdapter<String>(this, Resource.Layout.device_name);

            // Find and setup the ListView for paired devices
            var pairedListView = FindViewById<ListView>(Resource.Id.paired_devices);
            pairedListView.Adapter = pairedDevicesArrayAdapter;
            pairedListView.ItemClick += DeviceListView_ItemClick;

            // Find and set up the ListView for newly discovered devices
            var newDevicesListView = FindViewById<ListView>(Resource.Id.new_devices);
            newDevicesListView.Adapter = _newDevicesArrayAdapter;
            newDevicesListView.ItemClick += DeviceListView_ItemClick;

            // Register for broadcasts when a device is discovered
            _receiver = new DeviceDiscoveredReceiver(this);
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            RegisterReceiver(_receiver, filter);

            // Register for broadcasts when discovery has finished
            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(_receiver, filter);

            // Get the local Bluetooth adapter
            _btAdapter = BluetoothAdapter.DefaultAdapter;

            // Get a set of currently paired devices
            var pairedDevices = _btAdapter.BondedDevices;

            // If there are paired devices, add each on to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                FindViewById(Resource.Id.title_paired_devices).Visibility = ViewStates.Visible;
                foreach (var device in pairedDevices)
                {
                    pairedDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                }
            }
            else
            {
                var noDevices = Resources.GetText(Resource.String.none_paired);
                pairedDevicesArrayAdapter.Add(noDevices);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Make sure we're not doing discovery anymore
            _btAdapter?.CancelDiscovery();

            // Unregister broadcast listeners
            UnregisterReceiver(_receiver);
        }

        /// <summary>
        ///     Start device discovery with the BluetoothAdapter
        /// </summary>
        private void DoDiscovery()
        {
            // Indicate scanning in the title
            SetProgressBarIndeterminateVisibility(true);
            SetTitle(Resource.String.scanning);

            // Turn on sub-title for new devices
            FindViewById<View>(Resource.Id.title_new_devices).Visibility = ViewStates.Visible;

            // If we're already discovering, stop it
            if (_btAdapter.IsDiscovering)
            {
                _btAdapter.CancelDiscovery();
            }

            // Request discover from BluetoothAdapter
            var x = _btAdapter.StartDiscovery();
        }

        private void DeviceListView_ItemClick(Object sender, AdapterView.ItemClickEventArgs e)
        {
            _btAdapter.CancelDiscovery();

            // Get the device MAC address, which is the last 17 chars in the View
            var info = ((TextView) e.View).Text;
            var address = info.Substring(info.Length - 17);

            // Create the result intent and include MAC address.
            var intent = new Intent();
            intent.PutExtra(Constants.EXTRA_DEVICE_ADDRESS, address);

            SetResult(Result.Ok, intent);
            Finish();
        }

        public class DeviceDiscoveredReceiver : BroadcastReceiver
        {
            private readonly Activity chatActivity;

            public DeviceDiscoveredReceiver(Activity chat)
            {
                chatActivity = chat;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                var action = intent.Action;

                // When discovery finds a device
                if (action == BluetoothDevice.ActionFound)
                {
                    // Get the BluetoothDevice object from the Intent
                    var device = (BluetoothDevice) intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    // If it's already paired, skip it, because it's been listed already
                    if (device.BondState != Bond.Bonded)
                    {
                        ((ArrayAdapter) _newDevicesArrayAdapter).Add(device.Name + "\n" + device.Address);
                    }
                    // When discovery is finished, change the Activity title
                }
                else if (action == BluetoothAdapter.ActionDiscoveryFinished)
                {
                    chatActivity.SetProgressBarIndeterminateVisibility(false);
                    chatActivity.SetTitle(Resource.String.select_device);
                    if (_newDevicesArrayAdapter.Count == 0)
                    {
                        var noDevices = chatActivity.Resources.GetText(Resource.String.none_found);
                        ((ArrayAdapter) _newDevicesArrayAdapter).Add(noDevices);
                    }
                }
            }
        }
    }
}