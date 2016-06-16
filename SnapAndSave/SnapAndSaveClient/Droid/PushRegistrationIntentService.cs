using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.WindowsAzure.MobileServices;

namespace SnapAndSave.Droid
{
    [Service(Exported = false)]
    class PushRegistrationIntentService : IntentService
    {
        static object locker = new object();

        public PushRegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        Constants.GcmSenderId, GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("RegistrationIntentService", ex.Message);
                return;
            }
        }

        void SendRegistrationToAppServer(string token)
        {
            //Register with Mobile App
            try
            {
                var push = CouponService.CouponMobileServiceClient.GetPush();
                
                //const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";
                //JObject templates = new JObject();
                //templates["genericMessage"] = new JObject
                // {
                //     {"body", templateBodyGCM}
                // };
               
                //push.RegisterAsync(token, templates).Wait();
                push.RegisterAsync(token).Wait();
                Log.Info("Push Installation Id", push.InstallationId.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}