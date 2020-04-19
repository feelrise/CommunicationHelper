using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Boolean = System.Boolean;

namespace CommunicationHelper.App
{
    internal class WriteListener : Object, TextView.IOnEditorActionListener
    {
        private readonly BluetoothChatActivity _host;

        public WriteListener(BluetoothChatActivity host)
        {
            _host = host;
        }

        public Boolean OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.ImeNull && e.Action == KeyEventActions.Up)
                _host.SendMessage(new MessageEventArgs { Message = v.Text });
            return true;
        }
    }
}