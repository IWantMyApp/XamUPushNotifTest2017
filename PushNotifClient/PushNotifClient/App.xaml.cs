using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;

namespace PushNotifClient
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new PushNotifClient.MainPage();
		}

		/// <summary>
		/// Singleton. Azure Mobile Service Client.
		/// </summary>
		public MobileServiceClient MobileSvcClient => _mobileSvcClient.Value;

		Lazy<MobileServiceClient> _mobileSvcClient = new Lazy<MobileServiceClient>(() => {
			// The client gets initialized with the URL of the Mobile App Service, it will then
			// be able to automatically get the Notificatin Hub properties. The initialization of the
			// Notification Hub is in the platform specific projects (e.g. AppDelegate.cs).
			var client = new MobileServiceClient("http://rrpushnotificationsbackend.azurewebsites.net/");
			
			return client;
		}, true);

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
