using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CommunicationHelper.App
{
    public partial class BluetoothChatFragment
    {
        /// <summary>
        /// Handles messages that come back from the ChatService.
        /// </summary>
        public class ChatHandler : Handler
        {
            readonly BluetoothChatFragment _chatFragment;
            public ChatHandler(BluetoothChatFragment fragment)
            {
                _chatFragment = fragment;

            }
            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case Constants.MESSAGE_STATE_CHANGE:
                        switch (msg.What)
                        {
                            case BluetoothChatService.STATE_CONNECTED:
                                _chatFragment.SetStatus(_chatFragment.GetString(Resource.String.title_connected_to, _chatFragment.connectedDeviceName));
                                _chatFragment.conversationArrayAdapter.Clear();
                                break;
                            case BluetoothChatService.STATE_CONNECTING:
                                _chatFragment.SetStatus(Resource.String.title_connecting);
                                break;
                            case BluetoothChatService.STATE_LISTEN:
                                _chatFragment.SetStatus(Resource.String.not_connected);
                                break;
                            case BluetoothChatService.STATE_NONE:
                                _chatFragment.SetStatus(Resource.String.not_connected);
                                break;
                        }
                        break;
                    case Constants.MESSAGE_WRITE:
                        var writeBuffer = (byte[])msg.Obj;
                        var writeMessage = Encoding.ASCII.GetString(writeBuffer);
                        _chatFragment.conversationArrayAdapter.Add($"Me:  {writeMessage}");
                        break;
                    case Constants.MESSAGE_READ:
                        var readBuffer = (byte[])msg.Obj;
                        var readMessage = Encoding.ASCII.GetString(readBuffer);
                        _chatFragment.conversationArrayAdapter.Add($"{_chatFragment.connectedDeviceName}: {readMessage}");
                        break;
                    case Constants.MESSAGE_DEVICE_NAME:
                        _chatFragment.connectedDeviceName = msg.Data.GetString(Constants.DEVICE_NAME);
                        if (_chatFragment.Activity != null)
                        {
                            Toast.MakeText(_chatFragment.Activity, $"Connected to {_chatFragment.connectedDeviceName}.", ToastLength.Short).Show();
                        }
                        break;
                    case Constants.MESSAGE_TOAST:
                        break;
                }
            }
        }
    }
}