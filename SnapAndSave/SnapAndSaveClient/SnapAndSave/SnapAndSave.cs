using System;

using Xamarin.Forms;

namespace SnapAndSave
{
	public static class ViewModelLocator
	{
		

	}

	public class App : Application
	{
		public static CouponService CouponService = new CouponService ();

		public App ()
		{
			var content = new MainPage ();

			MainPage = new NavigationPage (content);
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

