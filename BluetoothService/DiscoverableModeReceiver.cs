using System;
using Android.Bluetooth;
using Android.Content;

namespace BluetoothService
{
    /// <summary>
    ///     Listen for when the device goes in and out of Bluetooth discoverability
    ///     mode, and will raise an Event.
    /// </summary>
    public class DiscoverableModeReceiver : BroadcastReceiver
    {
        public event EventHandler<BluetoothDiscoveryModeArgs> BluetoothDiscoveryModeChanged;

        public override void OnReceive(Context context, Intent intent)
        {
            var currentScanMode = intent.GetIntExtra(BluetoothAdapter.ExtraScanMode, -1);
            var previousScanMode = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousScanMode, -1);

            var inDiscovery = currentScanMode == (Int32) ScanMode.ConnectableDiscoverable;

            BluetoothDiscoveryModeChanged?.Invoke(this, new BluetoothDiscoveryModeArgs(inDiscovery));
        }
    }
}