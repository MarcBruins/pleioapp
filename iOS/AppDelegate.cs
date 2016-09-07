using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin;

namespace Pleioapp.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			Insights.Initialize (Constants.InsightsKey);

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
				//Xamarin.Calabash.Start();
			#endif

			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			// Get current device token
			var DeviceToken = deviceToken.Description;
			if (!string.IsNullOrWhiteSpace(DeviceToken)) {
				DeviceToken = DeviceToken.Trim('<').Trim('>').Replace(" ", "");
			}

			// Get previous device token
			var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

			// Has the token changed?
			if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
			{
				var service = new PushService ();
				service.SaveToken (DeviceToken);
				service.RegisterToken ();
			}
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			System.Diagnostics.Debug.WriteLine ("A problem occured during push notification registration " + error.LocalizedDescription);
		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			// @todo: convert NSDictionary to normal Dictionary and pass variables
			var data = new Dictionary<string, string> ();

			var app = (App) App.Current;
			app.pushService.ProcessPushNotification(data);
		}
	}
}

