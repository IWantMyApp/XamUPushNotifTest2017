using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PushNotifClient
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		public void Update(string text, string installationId)
		{
			lbl.Text = text;
			txt.Text = installationId;
		}
	}
}
