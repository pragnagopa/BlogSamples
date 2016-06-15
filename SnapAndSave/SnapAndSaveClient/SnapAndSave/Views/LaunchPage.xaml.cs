using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SnapAndSave
{
	public partial class LaunchPage : ContentPage
	{
		public LaunchPage ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{			
			base.OnAppearing ();

			InitializeAppAsync ().ContinueWith (HandleError, null, CancellationToken.None, 
			                                    TaskContinuationOptions.OnlyOnFaulted, 
			                                    TaskScheduler.FromCurrentSynchronizationContext());
		}

		private async Task InitializeAppAsync ()
		{
			var app = (App)Application.Current;
			await app.CouponService.InitializeAsync ();

			var viewModel = new CouponsPageViewModel (app.CouponService);
			var couponPage = new CouponListPage (viewModel);
			app.MainPage = new NavigationPage (couponPage);
		}

		private async Task HandleError (Task t, object state)
		{
			var message = t.Exception.InnerExceptions.First ().Message;
			await DisplayAlert ("Uh oh", message, "Yup, I'm screwed");
		}
	}
}

