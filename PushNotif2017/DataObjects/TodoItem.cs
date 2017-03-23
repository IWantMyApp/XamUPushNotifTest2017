using Microsoft.Azure.Mobile.Server;

namespace PushNotif2017.DataObjects
{
	public class TodoItem : EntityData
	{
		public string Text { get; set; }

		public bool Complete { get; set; }
	}
}