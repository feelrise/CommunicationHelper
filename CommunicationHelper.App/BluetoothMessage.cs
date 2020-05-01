using System;
using BluetoothService;

namespace CommunicationHelper.App
{
    public class BluetoothMessage : Java.Lang.Object
    {
        public String Message { get; set; }

        public String Sender { get; set; }

        public static implicit operator BluetoothMessage(HandlerResultEventArgs args)
        {
            return new BluetoothMessage(){Message = args.Message, Sender = args.Sender};
        }

        public override String ToString()
        {
            return $"{Sender}: {Message}";
        }
    }
}