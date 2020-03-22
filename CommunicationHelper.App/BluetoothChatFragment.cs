using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fragment = Android.Support.V4.App.Fragment;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BluetoothService;

namespace CommunicationHelper.App
{
    partial class BluetoothChatFragment : Fragment
    {
        ListView conversationView;
        EditText outEditText;
        Button sendButton;

        String connectedDeviceName = "";
        ArrayAdapter<String> conversationArrayAdapter;
        StringBuilder outStringBuffer;
        public BluetoothChatService chatService { get; set; }

        WriteListener writeListener;

        public BluetoothChatFragment()
        {   
            
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            writeListener = new WriteListener(this);
        }

        public override void OnStart()
        {
            base.OnStart();
            
            SetupChat();
        }

       

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.bluetooth_chat_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            conversationView = view.FindViewById<ListView>(Resource.Id.@in);
            outEditText = view.FindViewById<EditText>(Resource.Id.edit_text_out);
            sendButton = view.FindViewById<Button>(Resource.Id.button_send);
        }

        void SetupChat()
        {
            conversationArrayAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.message);
            conversationView.Adapter = conversationArrayAdapter;

            outEditText.SetOnEditorActionListener(writeListener);
            sendButton.Click += (sender, e) =>
            {
                var textView = View.FindViewById<TextView>(Resource.Id.edit_text_out);
                var msg = textView.Text;
                SendMessage(msg);
            };

            outStringBuffer = new StringBuilder("");
        }

        public void SendMessage(String message)
        {
            if (chatService.GetState() != BluetoothChatService.STATE_CONNECTED)
            {
                Toast.MakeText(Activity, Resource.String.not_connected, ToastLength.Long).Show();
                return;
            }

            if (message.Length > 0)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                chatService.Write(bytes);
                outStringBuffer.Clear();
                outEditText.Text = outStringBuffer.ToString();
            }
        }

        bool HasActionBar()
        {
            if (Activity == null)
            {
                return false;
            }
            if (Activity.ActionBar == null)
            {
                return false;
            }
            return true;
        }

        void SetStatus(int resId)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.SetSubtitle(resId);
            }
        }

        void SetStatus(string subTitle)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.Subtitle = subTitle;
            }
        }
    }
}