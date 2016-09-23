using System;
using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;
using Android.Util;
using Xamarin.Forms;

namespace Pleioapp.Droid
{
    [Service(Exported = false)]
    class RegistrationPushIntentService : IntentService
    {
        const string TAG = "RegistrationPushIntentService";

        IPushService pushService;

        static object locker = new object();

        public RegistrationPushIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    pushService = DependencyService.Get<IPushService>();
                    pushService.RegisterToken();
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, $"Failed to get a registration token:{e.Message}");
                return;
            }
        }
    }

    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    class PleioInstanceIDListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(RegistrationPushIntentService));
            StartService(intent);
        }
    }

}

