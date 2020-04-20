using System;
using Java.Util;

namespace BluetoothService
{
    public static class Constants
    {
        public const Int32 MESSAGE_STATE_CHANGE = 1;
        public const Int32 MESSAGE_READ = 2;
        public const Int32 MESSAGE_WRITE = 3;
        public const Int32 MESSAGE_DEVICE_NAME = 4;
        public const Int32 MESSAGE_TOAST = 5;

        public const String DEVICE_NAME = "device_name";
        public const String TOAST = "toast";
        public const string EXTRA_DEVICE_ADDRESS = "device_address";

        public const Int32 REQUEST_CONNECT_DEVICE_SECURE = 1;
        public const Int32 REQUEST_CONNECT_DEVICE_INSECURE = 2;
        public const Int32 REQUEST_ENABLE_BT = 3;

        public const String TAG = "BluetoothChatService";

        public const String NAME_SECURE = "BluetoothChatSecure";
        public const String NAME_INSECURE = "BluetoothChatInsecure";

        public static readonly UUID MY_UUID_SECURE = UUID.FromString("fa87c0d0-afac-11de-8a39-0800200c9a66");
        public static readonly UUID MY_UUID_INSECURE = UUID.FromString("8ce255c0-200a-11e0-ac64-0800200c9a66");

        public const Int32 STATE_NONE = 0; // we're doing nothing
        public const Int32 STATE_LISTEN = 1; // now listening for incoming connections
        public const Int32 STATE_CONNECTING = 2; // now initiating an outgoing connection
        public const Int32 STATE_CONNECTED = 3; // now connected to a remote device
    }
}