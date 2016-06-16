using Microsoft.Azure.Mobile.Server.Files;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SnapAndSaveService
{
    public class SnapAndSaveStorageProvider : AzureStorageProvider
    {
        public SnapAndSaveStorageProvider(string connectionString) : base(connectionString)
        {

        }

        public override async Task<IEnumerable<MobileServiceFile>> GetRecordFilesAsync(string tableName, string recordId, IContainerNameResolver containerNameResolver)
        {
            var containerNames = await containerNameResolver.GetRecordContainerNames(tableName, recordId);
            CloudBlobContainer container = GetContainer(containerNames.First());

            IEnumerable<IListBlobItem> blobs = await Task.Run(() => container.ListBlobs(prefix:recordId,useFlatBlobListing:true, blobListingDetails: BlobListingDetails.Metadata));

            return blobs.OfType<CloudBlockBlob>().Select(b => MobileServiceFile.FromBlobItem(b, tableName, recordId));

            //return blobs.OfType<CloudBlockBlob>();
        }
    }
}