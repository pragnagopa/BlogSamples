using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server.Files.Controllers;
using Microsoft.Azure.Mobile.Server.Files;
using System.Web.Http;
using SnapAndSaveService.DataObjects;
using System.Threading.Tasks;
using System.Configuration;

namespace SnapAndSaveService.Controllers
{
    [RoutePrefix("tables/coupon")]
    public class CouponStorageController : StorageController<Coupon>
    {
        CouponStorageController() : base(new SnapAndSaveStorageProvider(ConfigurationManager.ConnectionStrings[Constants.StorageConnectionStringName].ConnectionString))
        {

        }

        [HttpPost]
        [Route("{id}/StorageToken")]
        public async Task<IHttpActionResult> PostStorageTokenRequest(string id, StorageTokenRequest value)
        {
            StorageToken token = await GetStorageTokenAsync(id, value, new ContainerNameResolver());

            return Ok(token);
        }

        [HttpGet]
        [Route("{id}/MobileServiceFiles")]
        public async Task<IHttpActionResult> GetFiles(string id)
        {
            return Ok(await GetRecordFilesAsync(id, new ContainerNameResolver()));
        }

        [HttpDelete]
        [Route("{id}/MobileServiceFiles/{name}")]
        public Task Delete(string id, string name)
        {
            return base.DeleteFileAsync(id, name);
        }
    }
}