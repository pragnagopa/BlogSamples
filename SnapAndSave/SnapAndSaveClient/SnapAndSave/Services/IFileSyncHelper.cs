using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;

namespace SnapAndSave
{
	public interface IFileSyncHelper
	{
		Task UploadFileAsync<T> (IMobileServiceSyncTable<T> table, MobileServiceFile file, string filePath);
		Task DownloadFileAsync<T> (IMobileServiceSyncTable<T> table, MobileServiceFile file, string targetPath);

		IMobileServiceFileDataSource GetMobileServiceDataSource (string filePath);
	}
}

