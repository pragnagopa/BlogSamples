
using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

namespace SnapAndSave.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    class PushInstanceIDListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(PushRegistrationIntentService));
            StartService(intent);
        }
    }
}