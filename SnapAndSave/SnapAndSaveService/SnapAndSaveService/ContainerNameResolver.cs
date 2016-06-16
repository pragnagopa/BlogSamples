using Microsoft.Azure.Mobile.Server.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace SnapAndSaveService
{
    public class ContainerNameResolver : IContainerNameResolver
    {

        public const string DefaultContainerPrefix = "coupons";
        public Task<string> GetFileContainerNameAsync(string tableName, string recordId, string fileName)
        {
            return Task.FromResult($"{DefaultContainerPrefix}");
        }

        public Task<IEnumerable<string>> GetRecordContainerNames(string tableName, string recordId)
        {
            
            return Task.FromResult<IEnumerable<string>>(new[] { $"{DefaultContainerPrefix}" });
        }
    }
}