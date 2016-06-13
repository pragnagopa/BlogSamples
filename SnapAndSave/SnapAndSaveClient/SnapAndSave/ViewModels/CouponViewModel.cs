using System;
namespace SnapAndSave
{
	public class CouponViewModel
	{
		private Coupon coupon;

		public CouponViewModel (Coupon coupon)
		{
			this.coupon = coupon;
		}

		public string Description => coupon.Description;
		public DateTime Expiry => coupon.Expiry;

		public string PhotoUrl { get; set; }
	}
}

