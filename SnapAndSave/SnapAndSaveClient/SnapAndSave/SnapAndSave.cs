using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SnapAndSave
{
	public static class ViewModelLocator
	{
		

	}

	public class App : Application
	{
		public CouponService CouponService = new CouponService ();

		public App ()
		{
			//var viewModel = new CouponsPageViewModel (this.CouponService);
			//var startPage = new CouponListPage (viewModel);
			var startPage = new LaunchPage ();
			MainPage = startPage;
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

