using System.IO;
using Android.Bluetooth;
using Android.Util;
using Java.Lang;
using Byte = System.Byte;
using IOException = Java.IO.IOException;
using String = System.String;

namespace BluetoothService
{
    public partial class BluetoothService
    {
        private class ConnectedThread : Thread
        {
            private readonly BluetoothSocket _socket;
            private readonly Stream _inStream;
            private readonly Stream _outStream;
            private readonly BluetoothService _service;
            private readonly BluetoothDevice _bluetoothDevice;

            public ConnectedThread(BluetoothSocket socket, BluetoothService service, BluetoothDevice device, String socketType)
            {
                Log.Debug(Constants.TAG, $"create ConnectedThread: {socketType}");
                _socket = socket;
                _bluetoothDevice = device;
                _service = service;
                Stream tmpIn = null;
                Stream tmpOut = null;

                // Get the BluetoothSocket input and output streams
                try
                {
                    tmpIn = socket.InputStream;
                    tmpOut = socket.OutputStream;
                }
                catch (IOException e)
                {
                    Log.Error(Constants.TAG, "temp sockets not created", e);
                }

                _inStream = tmpIn;
                _outStream = tmpOut;
                service._state = Constants.STATE_CONNECTED;
            }

            public override void Run()
            {
                Log.Info(Constants.TAG, "BEGIN mConnectedThread");
                var buffer = new Byte[1024];

                // Keep listening to the InputStream while connected
                while (_service.GetState() == Constants.STATE_CONNECTED)
                {
                    try
                    {
                        // Read from the InputStream
                        var bytes = _inStream.Read(buffer, 0, buffer.Length);

                        // Send the obtained bytes to the UI Activity
                        _service._handler
                            .ObtainMessage(Constants.MESSAGE_READ, bytes, -1, buffer)
                            .SendToTarget();
                    }
                    catch (IOException e)
                    {
                        Log.Error(Constants.TAG, "disconnected", e);
                        _service.ConnectionLost();
                        break;
                    }
                }
            }

            /// <summary>
            ///     Write to the connected OutStream.
            /// </summary>
            /// <param name='buffer'>
            ///     The bytes to write
            /// </param>
            public void Write(Byte[] buffer)
            {
                try
                {
                    _outStream.Write(buffer, 0, buffer.Length);

                    // Share the sent message back to the UI Activity
                    _service._handler
                        .ObtainMessage(Constants.MESSAGE_WRITE, -1, -1, buffer)
                        .SendToTarget();
                }
                catch (IOException e)
                {
                    Log.Error(Constants.TAG, "Exception during write", e);
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