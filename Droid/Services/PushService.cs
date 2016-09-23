using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pleioapp.Droid;
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;

[assembly: Dependency(typeof(PushService))]
namespace Pleioapp.Droid
{
	public class PushService : IPushService
	{
		public void RequestToken() {
            return;
		}

		public void SetBadgeNumber(int number) {
            return;
		}

		public void SaveToken(string DeviceToken) {
            return;
		}

		public string GetToken() {
            var instance = InstanceID.GetInstance(Xamarin.Forms.Forms.Context);
            var token = instance.GetToken(Constants.GCMSenderId, GoogleCloudMessaging.InstanceIdScope);
            return token;
        }

		public async Task<bool> RegisterToken()
		{
			var app = (App)App.Current;
			var service = app.webService;
            var deviceId = Android.Provider.Settings.Secure.GetString(Xamarin.Forms.Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var token = GetToken();

            return await service.RegisterPush(deviceId, token, "gcm");
		}

		public Task<bool> DeregisterToken()
		{
			var app = (App)App.Current;
			var service = app.webService;
            var deviceId = Android.Provider.Settings.Secure.GetString(Xamarin.Forms.Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            return service.DeregisterPush(deviceId, "gcm");
		}

		public void ProcessPushNotification(Dictionary <string, string> data)
		{
			System.Diagnostics.Debug.WriteLine ("Received a push notification " + data.ToString());
			MessagingCenter.Send<Xamarin.Forms.Application> (App.Current, "refresh_menu");
		}
	}
}

