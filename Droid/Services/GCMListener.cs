
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
        const string TITLE = "PleioApp";

        public override void OnMessageReceived(string from, Bundle data)
        {
            var notificationBundle = data.Get("notification") as Bundle;

            if (notificationBundle != null)
            {
                var message = notificationBundle.Get("body");
                SendNotification(message.ToString());
            }
        }

        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                                                      .SetContentTitle(TITLE)
                                                      .SetContentText(message)
                                                      .SetSmallIcon(Resource.Drawable.icon)
                                                      .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
