using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;

namespace PushNotifClient.Droid
{
	[Activity (Label = "PushNotifClient", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			// Handle possible data accompanying notification message (if the app was started by tapping the
			// notification it will put data into the Extras).
			if (Intent.Extras != null)
			{
				foreach (var key in Intent.Extras.KeySet())
				{
					var value = Intent.Extras.GetString(key);
					Console.WriteLine("Key: {0} Value: {1}", key, value);
				}
			}

			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new PushNotifClient.App());

			// Uncomment to force a new token in case the app refuses to receive notifications.
			// This saves you from uninstalling and redeploying the app. Not sure if this is a side effect of
			// Xamarin when changing the app.
			Task.Run(() =>
			{
				Firebase.Iid.FirebaseInstanceId.Instance.DeleteInstanceId();
				var token = Firebase.Iid.FirebaseInstanceId.Instance.Token;
				Console.WriteLine("Token: " + token);
			});
		}
	}
}

