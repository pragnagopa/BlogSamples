using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SnapAndSave
{
	public class CouponsPageViewModel
	{
		public static IEnumerable<CouponViewModel> FakeCoupons { get; } = new List<CouponViewModel> {
			new CouponViewModel(new Coupon { Description = "JCPenney - 15% off", Expiry = new DateTime(2017,1,1) }) { PhotoUrl = "jcpenny.png" },
			new CouponViewModel(new Coupon { Description = "Target - 50% off", Expiry = new DateTime(2015,12,31) }) { PhotoUrl = "target-coupons.png" }, 
			new CouponViewModel(new Coupon { Description = "Quiznos - Free Cookie", Expiry = new DateTime(2013, 2, 14) }) { PhotoUrl = "quiznos.jpg" }
		};

		public ObservableCollection<CouponViewModel> Coupons { get; set; }

		public CouponsPageViewModel ()
		{
			
		}


	}
}

