using System;

namespace BluetoothService
{
    [Flags]
    public enum HandlerResultEnum
    {
        None = 0,
        StatusUpdated = 1,
        MessageAppeared = 2,
        Alert = 3,
        ClearChat = 4
    }

    public static class HandlerStatus
    {
        public static Boolean IsStatusUpdated(this HandlerResultEnum handlerResult)
        {
            return handlerResult.ContainsValue(HandlerResultEnum.StatusUpdated);
        }

        public static Boolean ContainsValue(this HandlerResultEnum handlerResult, HandlerResultEnum value)
        {
            return (handlerResult & value) == value;
        }
    }
}