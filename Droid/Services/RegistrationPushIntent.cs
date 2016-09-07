
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
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

                    //pushService.SaveToken(token);
                    pushService.RegisterToken();
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, "Failed to get a registration token");
                return;
            }
        }

        //void SendRegistrationToAppServer(string token)
        //{
        //    // Add custom implementation here as needed.
        //    var deviceId = Android.Provider.Settings.Secure.GetString(ContentResolver,Android.Provider.Settings.Secure.ANDROID_ID);
        //    return service.RegisterPush(deviceId, token, "apns");
        //}

        //void Subscribe(string token)
        //{
        //    var pubSub = GcmPubSub.GetInstance(this);
        //    pubSub.Subscribe(token, GCM_TOPIC, null);
        //}
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

    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class PleioGcmListenerService : GcmListenerService
    {
        const string TAG = "PleioGcmListenerService";
        const string DEFAULT_TITLE = "GCM Message";

        public override void OnMessageReceived(string from, Bundle data)
        {
            var pushService = DependencyService.Get<IPushService>();
            //todo: pushService.ProcessPushNotification(null);
            pushService.ProcessPushNotification(null);


            var message = data.GetString("message");
            Log.Debug(TAG, "From:    " + from);
            Log.Debug(TAG, "Message: " + message);
            SendNotification(message);
        }

        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                                                      .SetSmallIcon(Resource.Drawable.ic_tv_dark)
                                                      .SetContentTitle(DEFAULT_TITLE)
                                                      .SetContentText(message)
                                                      .SetAutoCancel(true)
                                                      .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}

