﻿using System;
using Foundation;
using UIKit;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;

namespace PushNotifClient.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		App formsApp;
		
		public override bool FinishedLaunching(UIApplication nativeApp, NSDictionary options)
		{
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			Xamarin.Forms.Forms.Init();

			this.formsApp = new PushNotifClient.App();
			LoadApplication(this.formsApp);

			// Guidelines for notifications: https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/Introduction.html#//apple_ref/doc/uid/TP40008194

			// App must ask the user for permission, no matter if we're using local or remote notifications.
			// There is also RegisterForRemoteNotificationTypes() but it is deprecated in iOS8.
			// NOTE: !Push notifications are not supported on the iOS Simulator!

			nativeApp.RegisterUserNotificationSettings(UIUserNotificationSettings.GetSettingsForTypes(
				UIUserNotificationType.Alert
				| UIUserNotificationType.Badge
				| UIUserNotificationType.Sound,
				null));
				
			// Request registration for remote notifications. This will talk to Apple's push server and the app will receive a token
			// when RegisteredForRemoteNotifications() gets called. This call is non-blocking.
			nativeApp.RegisterForRemoteNotifications();

			return base.FinishedLaunching(nativeApp, options);
		}
	

		/// <summary>
		/// After you call the registerForRemoteNotifications method of the UIApplication object,
		/// the app calls this method when device registration completes successfully.
		/// In your implementation of this method, connect with your push notification server and give the token to it.
		/// APNs pushes notifications only to the device represented by the token.
		/// The app might call this method in other rare circumstances, such as when the user launches an app after having
		/// restored a device from data that is not the device?s backup data. In this exceptional case, the app won?t know
		/// the new device?s token until the user launches it.
		/// </summary>
		/// <returns>The for remote notifications.</returns>
		/// <param name="application">Application.</param>
		/// <param name="deviceToken">Device token.</param>
		public async override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			if (deviceToken == null)
			{
				// Can happen in rare conditions e.g. after restoring a device.
				return;
			}

			// This is the template/payload used by iOS. It contains the "messageParam"
			// that will be replaced by our service.
			const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

			var templates = new JObject();
			templates["genericMessage"] = new JObject
			{
				{"body", templateBodyAPNS}
			};

			// Register the device with Notification Hub.
			// The type returned here is "Push" and can be found here:
			// The Mobile Client SDK is using the NotificationHub's REST API. 
			// The endpoint to create an installation can be found here:
			// https://msdn.microsoft.com/en-us/library/azure/mt621153.aspx
			// The Push class knows the Notification Hub it should talk to because
			// the Mobile Service Client gets initialized with the Mobile Service Backend's URL and the
			// service has a connection to the Notification Hub.
			var push = formsApp.MobileSvcClient.GetPush();

			// This can fail but for simplicity no error handling.
			await push.RegisterAsync(deviceToken, templates);
		}



		/// <summary>
		/// After you call the RegisterForRemoteNotifications() method of the UIApplication object,
		/// the app calls this method when there is an error in the registration process.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="error">Error.</param>
		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			Console.WriteLine($"Failed to register for remote notifications: {error.Description}");
		}

		/// <summary>
		/// Will be called if app received a remote notification.
		/// According to Apple's docs this is the preferred method to use instead of ReceivedRemoteNotification().
		/// See: https://developer.apple.com/library/ios/documentation/UIKit/Reference/UIApplicationDelegate_Protocol/#//apple_ref/occ/intfm/UIApplicationDelegate/application:didReceiveRemoteNotification:fetchCompletionHandler:
		/// Use this method to process incoming remote notifications for your app.
		/// Unlike the application:didReceiveRemoteNotification: method, which is called only when your app is running in the foreground,
		/// the system calls this method when your app is running in the foreground or background. In addition,
		/// if you enabled the remote notifications background mode, the system launches your app (or wakes it from the suspended state)
		/// and puts it in the background state when a remote notification arrives. However, the system does not automatically launch your app if the user has force-quit it.
		/// In that situation, the user must relaunch your app or restart the device before the system attempts to launch your app automatically again.
		/// </summary>
		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			// This will be called if the app is in the background/not running and if in the foreground.
			// However, it will not display a notification visually if the app is in the foreground.


			// Extract some data from the notifiation and display it using an alert view.
			NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

			var alert = string.Empty;
			if (aps.ContainsKey(new NSString("alert")))
			{
				alert = (aps[new NSString("alert")] as NSString).ToString();
			}

			// Show alert
			if (!string.IsNullOrEmpty(alert))
			{
				((App)App.Current).ShowNotification(alert);
			}
		
			// We must call the completion handler as soon as possible, max. after 30 seconds, otherwise the app gets terminated.
			// If we use notifications to download something, we would return "UIBackgroundFetchResult.NewData".
			completionHandler(UIBackgroundFetchResult.NoData);
		}

		/// <summary>
		/// If the notifications contains custom actions ("buttons"), this method will be called.
		/// The notification has to contain a "category" key in the payload.
		/// </summary>
		/// <returns>The action.</returns>
		/// <param name="application">Application.</param>
		/// <param name="actionIdentifier">Action identifier.</param>
		/// <param name="remoteNotificationInfo">Remote notification info.</param>
		/// <param name="completionHandler">Completion handler.</param>
		public override void HandleAction(UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler)
		{
			// For details see: https://developer.apple.com/library/ios/documentation/UIKit/Reference/UIApplicationDelegate_Protocol/index.html#//apple_ref/occ/intfm/UIApplicationDelegate/application:handleActionWithIdentifier:forRemoteNotification:completionHandler:
		}
	}
}
