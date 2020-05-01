using System;
using System.Text;
using Android.OS;
using BluetoothService;

namespace CommunicationHelper.App
{
    public class BluetoothMessageHandler : Handler
    {
        public event EventHandler<HandlerResultEventArgs> OnHandled;

        public override void HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case Constants.MESSAGE_STATE_CHANGE:
                    switch (msg.What)
                    {
                        case Constants.STATE_CONNECTED:
                            RaiseOnHandled(new HandlerResultEventArgs
                            {
                                Status =
                                    $"{Resource.String.title_connected_to}{msg.Data.GetString(Constants.DEVICE_NAME)}",
                                HandlerResult = HandlerResultEnum.ClearChat | HandlerResultEnum.StatusUpdated
                            });
                            break;
                        case Constants.STATE_CONNECTING:
                            RaiseOnHandled(new HandlerResultEventArgs
                            {
                                Status = Resource.String.title_connecting.ToString(),
                                HandlerResult = HandlerResultEnum.StatusUpdated
                            });
                            break;
                        case Constants.STATE_LISTEN:
                            RaiseOnHandled(new HandlerResultEventArgs
                            {
                                Status = Resource.String.not_connected.ToString(),
                                HandlerResult = HandlerResultEnum.StatusUpdated
                            });
                            break;
                        case Constants.STATE_NONE:
                            RaiseOnHandled(new HandlerResultEventArgs
                            {
                                Status = Resource.String.not_connected.ToString(),
                                HandlerResult = HandlerResultEnum.StatusUpdated
                            });
                            break;
                    }
                    break;
                case Constants.MESSAGE_WRITE:
                    var writeBuffer = (Byte[])msg.Obj;
                    var writeMessage = Encoding.UTF8.GetString(writeBuffer);
                    RaiseOnHandled(new HandlerResultEventArgs
                    { Sender = "Me", Message = writeMessage, HandlerResult = HandlerResultEnum.MessageAppeared });
                    break;
                case Constants.MESSAGE_READ:
                    var readBuffer = (Byte[])msg.Obj;
                    var readMessage = Encoding.UTF8.GetString(readBuffer);
                    var sender = msg.Data.GetString(Constants.DEVICE_NAME);
                    RaiseOnHandled(new HandlerResultEventArgs
                    {
                        Sender = sender,
                        Message = readMessage,
                        IsInput = true,
                        HandlerResult = HandlerResultEnum.MessageAppeared
                    });
                    break;
                case Constants.MESSAGE_DEVICE_NAME:
                    var connectedDevice = msg.Data.GetString(Constants.DEVICE_NAME);
                    RaiseOnHandled(new HandlerResultEventArgs
                    {
                        Alert = $"Connected to {connectedDevice}",
                        ConnectedDeviceName = connectedDevice,
                        HandlerResult = HandlerResultEnum.AlertRaised | HandlerResultEnum.NewDeviceConnected
                    });
                    break;
                case Constants.MESSAGE_TOAST:
                    break;
            }
        }

        protected virtual void RaiseOnHandled(HandlerResultEventArgs e)
        {
            OnHandled?.Invoke(this, e);
        }
    }
}