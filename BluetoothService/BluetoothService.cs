using System;
using System.Runtime.CompilerServices;
using Android.Bluetooth;
using Android.OS;
using Boolean = System.Boolean;
using Byte = System.Byte;
using String = System.String;

namespace BluetoothService
{
    public partial class BluetoothService
    {
        private readonly BluetoothAdapter _btAdapter;
        private readonly Handler _handler;
        private AcceptThread _secureAcceptThread;
        private AcceptThread _insecureAcceptThread;
        private ConnectThread _connectThread;
        private ConnectedThread _connectedThread;   
        private Int32 _state;
        private Int32 _newState;
        private readonly Object _lock = new Object();

        public BluetoothService(Handler handler)
        {
            _btAdapter = BluetoothAdapter.DefaultAdapter;
            _state = Constants.STATE_NONE;
            _newState = _state;
            _handler = handler;
        }

        /// <summary>
        ///     Return the current connection state.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Int32 GetState()
        {
            return _state;
        }

        // Start the chat service. Specifically start AcceptThread to begin a
        // session in listening (server) mode. Called by the Activity onResume()
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (_connectThread != null)
            {
                _connectThread.Cancel();
                _connectThread = null;
            }

            if (_connectedThread != null)
            {
                _connectedThread.Cancel();
                _connectedThread = null;
            }

            if (_secureAcceptThread == null)
            {
                _secureAcceptThread = new AcceptThread(this, true);
                _secureAcceptThread.Start();
            }
            if (_insecureAcceptThread == null)
            {
                _insecureAcceptThread = new AcceptThread(this, false);
                _insecureAcceptThread.Start();
            }

            UpdateUserInterfaceTitle();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connect(BluetoothDevice device, Boolean secure)
        {
            if (_state == Constants.STATE_CONNECTING)
            {
                if (_connectThread != null)
                {
                    _connectThread.Cancel();
                    _connectThread = null;
                }
            }

            // Cancel any thread currently running a connection
            if (_connectedThread != null)
            {
                _connectedThread.Cancel();
                _connectedThread = null;
            }

            // Start the thread to connect with the given device
            _connectThread = new ConnectThread(device, this, secure);
            _connectThread.Start();

            UpdateUserInterfaceTitle();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connected(BluetoothSocket socket, BluetoothDevice device, String socketType)
        {
            // Cancel the thread that completed the connection
            if (_connectThread != null)
            {
                _connectThread.Cancel();
                _connectThread = null;
            }

            // Cancel any thread currently running a connection
            if (_connectedThread != null)
            {
                _connectedThread.Cancel();
                _connectedThread = null;
            }


            if (_secureAcceptThread != null)
            {
                _secureAcceptThread.Cancel();
                _secureAcceptThread = null;
            }

            if (_insecureAcceptThread != null)
            {
                _insecureAcceptThread.Cancel();
                _insecureAcceptThread = null;
            }

            // Start the thread to manage the connection and perform transmissions
            _connectedThread = new ConnectedThread(socket, this, device, socketType);
            _connectedThread.Start();

            // Send the name of the connected device back to the UI Activity
            var msg = _handler.ObtainMessage(Constants.MESSAGE_DEVICE_NAME);
            var bundle = new Bundle();
            bundle.PutString(Constants.DEVICE_NAME, device.Name);
            msg.Data = bundle;
            _handler.SendMessage(msg);

            UpdateUserInterfaceTitle();
        }

        /// <summary>
        ///     Stop all threads.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (_connectThread != null)
            {
                _connectThread.Cancel();
                _connectThread = null;
            }

            if (_connectedThread != null)
            {
                _connectedThread.Cancel();
                _connectedThread = null;
            }

            if (_secureAcceptThread != null)
            {
                _secureAcceptThread.Cancel();
                _secureAcceptThread = null;
            }

            if (_insecureAcceptThread != null)
            {
                _insecureAcceptThread.Cancel();
                _insecureAcceptThread = null;
            }

            _state = Constants.STATE_NONE;
            UpdateUserInterfaceTitle();
        }

        public void Write(Byte[] @out)
        {
            // Create temporary object
            ConnectedThread connectedThread;
            // Synchronize a copy of the ConnectedThread
            lock (_lock)
            {
                if (_state != Constants.STATE_CONNECTED)
                {
                    return;
                }
                connectedThread = _connectedThread;
            }
            // Perform the write unsynchronized
            connectedThread.Write(@out);
        }

        /// <summary>
        ///     Indicate that the connection was lost and notify the UI Activity.
        /// </summary>
        public void ConnectionLost()
        {
            var msg = _handler.ObtainMessage(Constants.MESSAGE_TOAST);
            var bundle = new Bundle();
            bundle.PutString(Constants.TOAST, "Unable to connect device.");
            msg.Data = bundle;
            _handler.SendMessage(msg);

            _state = Constants.STATE_NONE;
            UpdateUserInterfaceTitle();
            Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateUserInterfaceTitle()
        {
            _state = GetState();
            _newState = _state;
            _handler.ObtainMessage(Constants.MESSAGE_STATE_CHANGE, _newState, -1).SendToTarget();
        }

        /// <summary>
        ///     Indicate that the connection attempt failed and notify the UI Activity.
        /// </summary>
        private void ConnectionFailed()
        {
            _state = Constants.STATE_LISTEN;

            var msg = _handler.ObtainMessage(Constants.MESSAGE_TOAST);
            var bundle = new Bundle();
            bundle.PutString(Constants.TOAST, "Unable to connect device");
            msg.Data = bundle;
            _handler.SendMessage(msg);
            Start();
        }
    }
}