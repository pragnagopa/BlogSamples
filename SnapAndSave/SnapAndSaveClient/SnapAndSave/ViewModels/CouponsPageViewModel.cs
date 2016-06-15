using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SnapAndSave
{
	public class CouponsPageViewModel
	{
		public static CouponsPageViewModel DesignTime { get; } = new CouponsPageViewModel (
			new List<CouponViewModel> {
				new CouponViewModel(new Coupon { Description = "JCPenney - 15% off", Expiry = new DateTime(2017,1,1) }) { PhotoUrl = "jcpenny.png" },
				new CouponViewModel(new Coupon { Description = "Target - 50% off", Expiry = new DateTime(2015,12,31) }) { PhotoUrl = "target.png" },
				new CouponViewModel(new Coupon { Description = "Quiznos - Free Cookie", Expiry = new DateTime(2013, 2, 14) }) { PhotoUrl = "quiznos.jpg" }
			});

		//public string SearchText { get; set; }
		public ObservableCollection<CouponViewModel> Coupons { get; set; }

		public Command SearchCommand { get; }
		public Command AddCouponCommand { get; }
		public Command SyncCommand { get; }

		private CouponService couponService;
		private IFileHelper fileHelper;

		public CouponsPageViewModel (IEnumerable<CouponViewModel> designTimeCoupons)
		{
			Coupons = new ObservableCollection<CouponViewModel> (designTimeCoupons);
		}

		public CouponsPageViewModel (CouponService couponService)
		{
			this.couponService = couponService;
			this.fileHelper = DependencyService.Get<IFileHelper> ();

			Coupons = new ObservableCollection<CouponViewModel> ();
			this.SearchCommand = new Command (async (arg) => { await SearchCouponsAsync ((string)arg); });
			this.AddCouponCommand = new Command (async (arg) => { await AddCouponAsync (); });
			this.SyncCommand = new Command (async (arg) => { await SyncAsync (); }); 
		}

		private async Task SearchCouponsAsync (string searchInput)
		{			
			var results = await couponService.SearchCoupons (searchInput);
			Coupons.Clear ();
			foreach (var c in results) {
				var viewModel = new CouponViewModel (c);
				var couponImageName = await couponService.GetCouponImageName (c);
				viewModel.PhotoUrl = fileHelper.GetLocalFilePath (c.Id, couponImageName);
				Coupons.Add (viewModel);
			}
		}

		private async Task AddCouponAsync ()
		{
			var photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync ();

			if (photo != null) {
				var coupon = new Coupon { Id = Guid.NewGuid ().ToString () };
				coupon.Description = "Placeholder";

				var targetPath = fileHelper.CopyFileToAppDirectory (coupon.Id, photo.Path);
				await couponService.InsertCoupon (coupon, Path.GetFileName(targetPath));
				await SearchCouponsAsync ("");
			}

		}

		private async Task SyncAsync ()
		{
			await couponService.SyncAsync ();
			await SearchCouponsAsync ("");
		}


	}
}

