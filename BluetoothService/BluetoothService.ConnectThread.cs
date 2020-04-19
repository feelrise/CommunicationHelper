using Android.Bluetooth;
using Android.Util;
using Java.IO;
using Java.Lang;
using Boolean = System.Boolean;
using Object = System.Object;
using String = System.String;

namespace BluetoothService {
    public partial class BluetoothService {

        protected class ConnectThread : Thread
        {
            private readonly BluetoothSocket _socket;
            private readonly BluetoothDevice _device;
            private readonly BluetoothService _service;
            private readonly String _socketType;
            private readonly Object _lock = new Object();


            public ConnectThread(BluetoothDevice device, BluetoothService service, Boolean secure)
            {
                this._device = device;
                this._service = service;
                BluetoothSocket tmp = null;
                _socketType = secure ? "Secure" : "Insecure";

                try
                {
                    tmp = secure
                        ? device.CreateRfcommSocketToServiceRecord(Constants.MY_UUID_SECURE)
                        : device.CreateInsecureRfcommSocketToServiceRecord(Constants.MY_UUID_INSECURE);
                }
                catch (IOException e)
                {
                    Log.Error(Constants.TAG, "create() failed", e);
                }
                _socket = tmp;
                service._state = Constants.STATE_CONNECTING;
            }

            public override void Run()
            {
                Name = $"ConnectThread_{_socketType}";

                // Always cancel discovery because it will slow down connection
                lock (_lock)
                {
                    _service._btAdapter.CancelDiscovery();
                }

                // Make a connection to the BluetoothSocket
                try
                {
                    // This is a blocking call and will only return on a
                    // successful connection or an exception
                    _socket.Connect();
                }
                catch (IOException)
                {
                    // Close the socket
                    try
                    {
                        _socket.Close();
                    }
                    catch (IOException e2)
                    {
                        Log.Error(Constants.TAG, $"unable to close() {_socketType} socket during connection failure.",
                            e2);
                    }

                    // Start the service over to restart listening mode
                    lock (_lock)
                    {
                        _service.ConnectionFailed();
                    }
                    return;
                }

                // Reset the ConnectThread because we're done
                lock (_lock)
                {
                    _service._connectThread = null;
                }

                // Start the connected thread
                lock (_lock)
                {
                    _service.Connected(_socket, _device, _socketType);
                }
            }

            public void Cancel()
            {
                try
                {
                    _socket.Close();
                }
                catch (IOException e)
                {
                    Log.Error(Constants.TAG, "close() of connect socket failed", e);
                }
            }
        }
    }
}