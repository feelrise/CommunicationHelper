using System;

namespace CommunicationHelper.App
{
    public static class Constants
    {
        public const Int32 MESSAGE_STATE_CHANGE = 1;
        public const Int32 MESSAGE_READ = 2;
        public const Int32 MESSAGE_WRITE = 3;
        public const Int32 MESSAGE_DEVICE_NAME = 4;
        public const Int32 MESSAGE_TOAST = 5;

        public const String DEVICE_NAME = "device_name";
        public const String TOAST = "toast";
    }
}