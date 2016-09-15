
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace Pleioapp.Droid
{
   
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class GCMListener : GcmListenerService
    {
        const string TAG = "PleioGcmListenerService";
        const string DEFAULT_TITLE = "GCM Message";

        public override void OnMessageReceived(string from, Bundle data)
        {
            var pushService = DependencyService.Get<IPushService>();
            //   //todo: pushService.ProcessPushNotification(null);
            //   var message = data.GetString("message");

            ////   pushService.ProcessPushNotification(new Dictionary<string, string>());

            //   Log.Debug(TAG, "From:    " + from);
            //   Log.Debug(TAG, "Message: " + message);
            //   SendNotification(message);
        }

        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                                                      .SetPriority(10)
                                                      .SetContentTitle("TITLE")
                                                      .SetContentText("TEKST")
                                                      .SetAutoCancel(true)
                                                      .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
