using System;

namespace BluetoothService
{
    public class HandlerResultEventArgs : EventArgs
    {
        public String Message { get; set; }

        public String Status { get; set; }

        public String Alert { get; set; }

        public HandlerResultEnum HandlerResult { get; set; }
    }
}