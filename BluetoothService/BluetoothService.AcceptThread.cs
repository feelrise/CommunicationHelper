using Android.Bluetooth;
using Android.Util;
using Java.IO;
using Java.Lang;
using Boolean = System.Boolean;
using Object = System.Object;
using String = System.String;

namespace BluetoothService {
    public partial class BluetoothService {
 
        private class AcceptThread : Thread
        {
            // The local server socket
            private readonly BluetoothServerSocket _serverSocket;
            private readonly String _socketType;
            private readonly BluetoothService _service;
            private readonly Object _lock = new Object();

            public AcceptThread(BluetoothService service, Boolean secure)
            {
                BluetoothServerSocket tmp = null;
                _socketType = secure ? "Secure" : "Insecure";
                this._service = service;

                try
                {
                    if (secure)
                    {
                        tmp = service._btAdapter.ListenUsingRfcommWithServiceRecord(Constants.NAME_SECURE,
                            Constants.MY_UUID_SECURE);
                    }
                    else
                    {
                        tmp = service._btAdapter.ListenUsingInsecureRfcommWithServiceRecord(Constants.NAME_INSECURE,
                            Constants.MY_UUID_INSECURE);
                    }
                }
                catch (IOException e)
                {
                    Log.Error(Constants.TAG, "listen() failed", e);
                }
                _serverSocket = tmp;
                service._state = Constants.STATE_LISTEN;
            }

            public override void Run()
            {
                Name = $"AcceptThread_{_socketType}";

                lock (_lock)
                {
                    while (_service.GetState() != Constants.STATE_CONNECTED)
                    {
                        BluetoothSocket socket;
                        try
                        {
                            socket = _serverSocket.Accept();
                        }
                        catch (IOException e)
                        {
                            Log.Error(Constants.TAG, "accept() failed", e);
                            break;
                        }

                        if (socket == null)
                        {
                            continue;
                        }
                        {
                            switch (_service.GetState())
                            {
                                case Constants.STATE_LISTEN:
                                case Constants.STATE_CONNECTING:
                                    // Situation normal. Start the connected thread.
                                    _service.Connected(socket, socket.RemoteDevice, _socketType);
                                    break;
                                case Constants.STATE_NONE:
                                case Constants.STATE_CONNECTED:
                                    try
                                    {
                                        socket.Close();
                                    }
                                    catch (IOException e)
                                    {
                                        Log.Error(Constants.TAG, "Could not close unwanted socket", e);
                                    }
                                    break;
                            }
                        }

                    }
                }
            }

            public void Cancel()
            {
                try
                {
                    _serverSocket.Close();
                }
                catch (IOException e)
                {
                    Log.Error(Constants.TAG, "close() of server failed", e);
                }
            }
        }
    }
}