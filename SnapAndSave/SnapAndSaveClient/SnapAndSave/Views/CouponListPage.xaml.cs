using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SnapAndSave
{
	public partial class CouponListPage : ContentPage
	{
		private CouponsPageViewModel viewModel;

		public CouponListPage (CouponsPageViewModel viewModel)
		{
			InitializeComponent ();
			this.BindingContext = this.viewModel = viewModel;
		}

		protected override void OnAppearing ()
		{
			viewModel.SearchCommand.Execute ("");
			base.OnAppearing ();
		}

		void Handle_SearchTextChanged (object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			viewModel.SearchCommand.Execute (e.NewTextValue);
		}
	}
}

