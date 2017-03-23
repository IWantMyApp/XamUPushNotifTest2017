using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace PushNotif2017.Controllers
{
	[RoutePrefix("notifications")]
	public class NotificationsController : ApiController
	{
		[HttpPost]
		[Route("")]
		public async Task<IHttpActionResult> SendNotificationAsync([FromBody] string message)
		{
			// Get the settings for the server project.
			HttpConfiguration config = this.Configuration;
			var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

			// Get the Notification Hubs credentials for the Mobile App.
			string notificationHubName = settings.NotificationHubName;
			string notificationHubConnection = settings.Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

			// Create a new Notification Hub client.
			var hub = NotificationHubClient.CreateClientFromConnectionString(
				notificationHubConnection,
				notificationHubName, 
				// Don't use this in RELEASE builds. The number of devices is limited.
				// If TRUE, the send method will return the devices a message was
				// delivered to.
				enableTestSend: true);
			
			// Sending the message so that all template registrations that contain "messageParam"
			// will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
			var templateParams = new Dictionary<string, string>
			{
				["messageParam"] = message
			};
			
			try
			{
				// Send the push notification and log the results.
				var result = await hub.SendTemplateNotificationAsync(templateParams).ConfigureAwait(false);

				// Write the success result to the logs.
				config.Services.GetTraceWriter().Info(result.State.ToString());
			}
			catch (Exception ex)
			{
				// Write the failure result to the logs.
				config.Services.GetTraceWriter().Error(ex.Message, null, "Push.SendAsync Error");
				return BadRequest(ex.Message);
			}

			return Ok();
		}
	}
}
