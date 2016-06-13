using System;

using Xamarin.Forms;

namespace SnapAndSave
{
	public class App : Application
	{
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

