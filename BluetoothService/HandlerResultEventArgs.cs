using System;

namespace BluetoothService
{
    public class HandlerResultEventArgs : EventArgs
    {
        public String Message { get; set; }

        public Boolean IsInput { get; set; }

        public String Sender { get; set; }

        public String ConnectedDeviceName { get; set; }

        public String Status { get; set; }

        public String Alert { get; set; }

        public HandlerResultEnum HandlerResult { get; set; }
    }
}