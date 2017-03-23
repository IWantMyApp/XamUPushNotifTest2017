using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PushNotif2017.Startup))]

namespace PushNotif2017
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureMobileApp(app);
		}
	}
}