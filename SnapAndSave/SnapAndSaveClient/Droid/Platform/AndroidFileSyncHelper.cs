using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;

[assembly: Xamarin.Forms.Dependency (typeof (SnapAndSave.Droid.AndroidFileSyncHelper))]
namespace SnapAndSave.Droid
{

	public class AndroidFileSyncHelper : IFileSyncHelper
	{
		public async Task DownloadFileAsync<T> (IMobileServiceSyncTable<T> table, MobileServiceFile file, string targetPath)
		{
			await table.DownloadFileAsync (file, targetPath);
		}

		public async Task UploadFileAsync<T> (IMobileServiceSyncTable<T> table, MobileServiceFile file, string filePath)
		{
			await table.UploadFileAsync (file, filePath);
		}

		public IMobileServiceFileDataSource GetMobileServiceDataSource (string filePath)
		{
			return new PathMobileServiceFileDataSource (filePath);
		}
	}
}

