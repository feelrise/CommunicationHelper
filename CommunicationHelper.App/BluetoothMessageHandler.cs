using System;
using System.Text;
using Android.OS;
using Android.Widget;
using BluetoothService;

namespace CommunicationHelper.App
{
    public class BluetoothMessageHandler : Handler
    {   
        private readonly ChatFragment _chatFragment;

        public BluetoothMessageHandler(ChatFragment fragment)
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
                        case Constants.STATE_CONNECTED:
                            _chatFragment.SetStatus(_chatFragment.GetString(Resource.String.title_connected_to,
                                msg.Data.GetString(Constants.DEVICE_NAME)));
                            _chatFragment.ConversationArrayAdapter.Clear();
                            break;
                        case Constants.STATE_CONNECTING:
                            _chatFragment.SetStatus(Resource.String.title_connecting);
                            break;
                        case Constants.STATE_LISTEN:
                            _chatFragment.SetStatus(Resource.String.not_connected);
                            break;
                        case Constants.STATE_NONE:
                            _chatFragment.SetStatus(Resource.String.not_connected);
                            break;
                        default:
                            break;
                    }
                    break;
                case Constants.MESSAGE_WRITE:
                    var writeBuffer = (Byte[]) msg.Obj;
                    var writeMessage = Encoding.UTF8.GetString(writeBuffer);
                    _chatFragment.ConversationArrayAdapter.Add($"Me:  {writeMessage}");
                    break;
                case Constants.MESSAGE_READ:
                    var readBuffer = (Byte[]) msg.Obj;
                    var readMessage = Encoding.UTF8.GetString(readBuffer);
                    _chatFragment.ConversationArrayAdapter.Add($"{msg.Data.GetString(Constants.DEVICE_NAME)}: {readMessage}");
                    break;
                case Constants.MESSAGE_DEVICE_NAME:
                    if (_chatFragment.Activity != null)
                    {
                        Toast.MakeText(_chatFragment.Activity, $"Connected to {msg.Data.GetString(Constants.DEVICE_NAME)}.",
                            ToastLength.Short).Show();
                    }
                    break;
                case Constants.MESSAGE_TOAST:
                    break;
                default:
                    break;
            }
        }
    }
}