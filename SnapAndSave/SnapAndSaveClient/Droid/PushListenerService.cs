using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;

namespace SnapAndSave.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class PushListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            var message = data.GetString("message");
            Log.Debug("PushListenerService", "From:    " + from);
            Log.Debug("PushListenerService", "Message: " + message);
            SendNotification(message);
        }

        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("GCM Message")
                .SetContentText(message)
                .SetDefaults(NotificationDefaults.Sound)
                .SetPriority(1)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}