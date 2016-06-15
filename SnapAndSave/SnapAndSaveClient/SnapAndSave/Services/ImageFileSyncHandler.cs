using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;

namespace SnapAndSave
{
	public class ImageFileSyncHandler<T> : IFileSyncHandler where T: class
	{
		private readonly IMobileServiceSyncTable<T> couponTable;
		private readonly IFileSyncHelper fileSyncHelper;
		private readonly IFileHelper fileHelper;

		public ImageFileSyncHandler (IMobileServiceSyncTable<T> couponTable)
		{
			this.couponTable = couponTable;
			this.fileSyncHelper = DependencyService.Get<IFileSyncHelper> ();
			this.fileHelper = DependencyService.Get<IFileHelper> ();
		}

		public Task<IMobileServiceFileDataSource> GetDataSource (MobileServiceFileMetadata metadata)
		{
			var filePath = fileHelper.GetLocalFilePath (metadata.ParentDataItemId, metadata.FileName);
			return Task.FromResult (this.fileSyncHelper.GetMobileServiceDataSource (filePath));
		}

		public async Task ProcessFileSynchronizationAction (MobileServiceFile file, FileSynchronizationAction action)
		{
			if (action == FileSynchronizationAction.Delete) {
				fileHelper.DeleteLocalFile (file);
			} else {
				var filepath = fileHelper.GetLocalFilePath (file.ParentId, file.Name);
				await this.fileSyncHelper.DownloadFileAsync (couponTable, file, filepath);
			}
		}
	}
}

