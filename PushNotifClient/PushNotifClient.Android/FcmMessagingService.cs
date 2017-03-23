using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace PushNotifClient.Droid
{
	// This service is only required for certain scenarios, as describe here: https://developers.google.com/cloud-messaging/android/android-migrate-fcm#migrate_your_gcmlistenerservice
	// A service extending GcmListenerService is now required only for the following use cases:
	// receiving messages with notification payload while the application is in foreground
	// receiving messages with data payload only
	// receiving errors in case of upstream message failures.
	// If you don't use these features, and you only care about displaying notifications messages when the app is not in the foreground, you can completely remove this service.
	[Service(Exported = true), IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
	public class FcmMessagingService : FirebaseMessagingService
	{
		public override void OnMessageReceived(RemoteMessage message)
		{
			base.OnMessageReceived(message);

			// Extract the message. The line commented out works for standard message formats.
			//string msg = message.GetNotification().Body;
			string msg = message.Data["msg"];

			Console.WriteLine("Received: " + msg);
		}
	}
}