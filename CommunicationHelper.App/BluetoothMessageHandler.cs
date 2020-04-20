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
                            RaiseOnHandled(new HandlerResultEventArgs()
                            {
                                Status = $"{Resource.String.title_connected_to}{msg.Data.GetString(Constants.DEVICE_NAME)}",
                                HandlerResult = HandlerResultEnum.ClearChat & HandlerResultEnum.ClearChat
                            });
                            break;
                        case Constants.STATE_CONNECTING:
                            RaiseOnHandled(new HandlerResultEventArgs()
                            {
                                Status = Resource.String.title_connecting.ToString(),
                                HandlerResult = HandlerResultEnum.StatusUpdated
                            });
                            break;
                        case Constants.STATE_LISTEN:
                            RaiseOnHandled(new HandlerResultEventArgs()
                            {
                                Status = Resource.String.not_connected.ToString(),
                                HandlerResult = HandlerResultEnum.StatusUpdated
                            });
                            break;
                        case Constants.STATE_NONE:
                            RaiseOnHandled(new HandlerResultEventArgs()
                            {
                                Status = Resource.String.not_connected.ToString(),
                                HandlerResult = HandlerResultEnum.StatusUpdated
                            });
                            break;
                        default:
                            break;
                    }
                    break;
                case Constants.MESSAGE_WRITE:
                    var writeBuffer = (Byte[])msg.Obj;
                    var writeMessage = Encoding.UTF8.GetString(writeBuffer);
                    RaiseOnHandled(new HandlerResultEventArgs() { Message = $"Me:  {writeMessage}", HandlerResult = HandlerResultEnum.MessageAppeared });
                    break;
                case Constants.MESSAGE_READ:
                    var readBuffer = (Byte[])msg.Obj;
                    var readMessage = Encoding.UTF8.GetString(readBuffer);
                    RaiseOnHandled(new HandlerResultEventArgs()
                    {
                        Message = $"{msg.Data.GetString(Constants.DEVICE_NAME)}: {readMessage}",
                        HandlerResult = HandlerResultEnum.MessageAppeared
                    });
                    break;
                case Constants.MESSAGE_DEVICE_NAME:
                    RaiseOnHandled(new HandlerResultEventArgs()
                    {
                        Alert = $"Connected to {msg.Data.GetString(Constants.DEVICE_NAME)}",
                        HandlerResult = HandlerResultEnum.AlertRaised
                    });
                    break;
                case Constants.MESSAGE_TOAST:
                    break;
                default:
                    break;
            }
        }

        protected virtual void RaiseOnHandled(HandlerResultEventArgs e)
        {
            OnHandled?.Invoke(this, e);
        }
    }
}