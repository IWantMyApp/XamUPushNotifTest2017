using Android.App;
using Android.Content;
using Android.OS;
using Firebase.Iid;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PushNotifClient.Droid
{
	/// <summary>
	/// The service will be used if we get get informed about new token assignment from Firebase CM.
	/// It will be triggered by Android; we don't start it directly.
	/// For details on when and how this will be called: https://developers.google.com/instance-id/guides/android-implementation
	/// </summary>
	[Preserve]
	[Service]
	[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
	public class FcmInstanceIdListenerService : FirebaseInstanceIdService
	{
		public override void OnTokenRefresh()
		{
			var refreshedToken = FirebaseInstanceId.Instance.Token;

			var formsApp = (App)App.Current;
			
			var push = formsApp.MobileSvcClient.GetPush();

			// IntentService uses queued worker threads. To do something on the UI we must invoke on the main thread.
			var handler = new Handler(Looper.MainLooper);
			handler.Post(() =>
			{
				Register(push, refreshedToken, null);
			});
		}

		public async void Register(Push push, string token, IEnumerable<string> tags)
		{
			try
			{
				// Formats: https://firebase.google.com/docs/cloud-messaging/concept-options
				// The "notification" format will automatically displayed in the notification center if the 
				// app is not in the foreground.
				const string templateBodyGCM =
					"{" +
						"\"notification\" : {" +
						"\"body\" : \"$(messageParam)\"," +
	  					"\"title\" : \"Xamarin University\"," +
						"\"icon\" : \"myicon\" }" +
					"}";

				var templates = new JObject();
				templates["genericMessage"] = new JObject
				{
					{"body", templateBodyGCM}
				};

				await push.RegisterAsync(token, templates);

				// Push object contains installation ID afterwards.
				Console.WriteLine(push.InstallationId.ToString());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Debugger.Break();
			}
		}
	}
}