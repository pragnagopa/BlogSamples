using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Media;
using Android.Gms.Common;

namespace SnapAndSave.Droid
{
	[Activity (Label = "SnapAndSave.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init ();
			CrossMedia.Current.Initialize ();

			LoadApplication (new App ());

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(PushRegistrationIntentService));
                StartService(intent);
            }
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    System.Diagnostics.Debug.WriteLine(GoogleApiAvailability.Instance.GetErrorString(resultCode));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Sorry, this device is not supported for Gcm");
                    Finish();
                }
                return false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Google Play Services is available.");
                return true;
            }
        }
    }
}

