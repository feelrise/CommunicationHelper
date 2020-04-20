using System;

namespace BluetoothService
{
    [Flags]
    public enum HandlerResultEnum
    {
        None = 0,
        StatusUpdated = 1,
        MessageAppeared = 2,
        AlertRaised = 3,
        ClearChat = 4
    }

    public static class HandlerStatus
    {
        public static Boolean IsStatusUpdated(this HandlerResultEnum handlerResult)
        {
            return handlerResult.ContainsValue(HandlerResultEnum.StatusUpdated);
        }

        public static Boolean IsMessageAppeared(this HandlerResultEnum handlerResult)
        {
            return handlerResult.ContainsValue(HandlerResultEnum.MessageAppeared);
        }

        public static Boolean IsAlertRaised(this HandlerResultEnum handlerResult)
        {
            return handlerResult.ContainsValue(HandlerResultEnum.AlertRaised);
        }

        public static Boolean ClearChat(this HandlerResultEnum handlerResult)
        {
            return handlerResult.ContainsValue(HandlerResultEnum.ClearChat);
        }

        public static Boolean ContainsValue(this HandlerResultEnum handlerResult, HandlerResultEnum value)
        {
            return (handlerResult & value) == value;
        }
    }
}