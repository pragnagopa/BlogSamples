using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.Files;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.Eventing;

namespace SnapAndSave
{
	public class CouponService
	{
		private static MobileServiceClient mobileServiceClient = new MobileServiceClient (Constants.ApplicationURL);
		private ImageFileSyncHandler<Coupon> fileSyncHandler;
		private IMobileServiceSyncTable<Coupon> couponTable;

        public static MobileServiceClient CouponMobileServiceClient
        {
            get
            {
                if(mobileServiceClient == null)
                {
                    mobileServiceClient = new MobileServiceClient(Constants.ApplicationURL);
                }
                return mobileServiceClient;
            }
        }
		public async Task InitializeAsync ()
		{
			var store = new MobileServiceSQLiteStore (Constants.LocalDb);
			store.DefineTable<Coupon> ();

			this.couponTable = mobileServiceClient.GetSyncTable<Coupon> ();
			this.fileSyncHandler = new ImageFileSyncHandler<Coupon> (couponTable);

			mobileServiceClient.InitializeFileSyncContext (fileSyncHandler, store);
			await mobileServiceClient.SyncContext.InitializeAsync (store, StoreTrackingOptions.AllNotificationsAndChangeDetection);

			mobileServiceClient.EventManager.Subscribe<IMobileServiceEvent> (x => System.Diagnostics.Debug.WriteLine (x.Name));

			await SyncAsync ();
            
			//var coupon = new Coupon { Id = Guid.NewGuid ().ToString (), Description = "Test" };
			//await couponTable.InsertAsync (coupon);

			//coupon = new Coupon { Id = Guid.NewGuid ().ToString (), Description = "Target" };
			//await couponTable.InsertAsync (coupon);

			//coupon = new Coupon { Id = Guid.NewGuid ().ToString (), Description = "Subway" };
			//await couponTable.InsertAsync (coupon);

			//throw new Exception ("here we go");

		}
        
		public async Task<IEnumerable<Coupon>> SearchCoupons (string searchInput)
		{
			var query = couponTable.CreateQuery ();

			if (!string.IsNullOrEmpty (searchInput)) {
				query = query.Where (c => c.Description.ToUpper ().Contains (searchInput.ToUpper ()));
			}

			var results = await query.ToEnumerableAsync ();
			return results;
		}

		public async Task<string> GetCouponImageName (Coupon coupon)
		{
			var files = await couponTable.GetFilesAsync (coupon);
			var file = files.FirstOrDefault ();

			if (file != null)
				return file.Name;
			else
				return "";
		}

		public async Task InsertCoupon (Coupon coupon, string filename)
		{
			await couponTable.InsertAsync (coupon);
			await couponTable.AddFileAsync (coupon, filename);
		}

		public async Task SyncAsync ()
		{
			await mobileServiceClient.SyncContext.PushAsync ();
			await couponTable.PushFileChangesAsync ();

			await couponTable.PullAsync ("allcoupons", couponTable.CreateQuery ());
			await fileSyncHandler.DownloadsComplete ();
		}
	}
}

