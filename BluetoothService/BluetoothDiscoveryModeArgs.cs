using System;

namespace BluetoothService
{
    public class BluetoothDiscoveryModeArgs : EventArgs
    {
        public BluetoothDiscoveryModeArgs(Boolean inDiscoveryMode)
        {
            InDiscoveryMode = inDiscoveryMode;
        }

        public Boolean InDiscoveryMode { get; }
    }
}