using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin;
using Android.Gms.Common;

namespace Pleioapp.Droid
{
	[Activity (Label = "Pleioapp.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			Insights.Initialize (Constants.InsightsKey, this.ApplicationContext);

			LoadApplication (new App ());

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationPushIntentService));
                StartService(intent);
            }
		}

        public bool IsPlayServicesAvailable()
        {
            //TODO: log notification
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Toast.MakeText(this,GoogleApiAvailability.Instance.GetErrorString(resultCode),ToastLength.Long).Show();
                else
                {
                    Toast.MakeText(this, "Sorry, this device is not supported", ToastLength.Long).Show();
                    Finish();
                }
                return false;
            }
            //Toast.MakeText(this, "Google play services not available", ToastLength.Long).Show();
            return true;
        }
	}
}

