using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace CommunicationHelper.App
{
    class WriteListener : Java.Lang.Object, TextView.IOnEditorActionListener
    {
        BluetoothChatFragment host;
        public WriteListener(BluetoothChatFragment frag)
        {
            host = frag;
        }
        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.ImeNull && e.Action == KeyEventActions.Up)
            {
                host.SendMessage(v.Text);
            }
            return true;
        }
    }
}