using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using SnapAndSaveService.DataObjects;
using SnapAndSaveService.Models;

namespace SnapAndSaveService.Controllers
{
    public class CouponController : TableController<Coupon>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            SnapAndSaveContext context = new SnapAndSaveContext();
            DomainManager = new EntityDomainManager<Coupon>(context, Request);
        }

        // GET tables/Coupon
        public IQueryable<Coupon> GetAllCoupon()
        {
            return Query(); 
        }

        // GET tables/Coupon/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Coupon> GetCoupon(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Coupon/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Coupon> PatchCoupon(string id, Delta<Coupon> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Coupon
        public async Task<IHttpActionResult> PostCoupon(Coupon item)
        {
            Coupon current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Coupon/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCoupon(string id)
        {
             return DeleteAsync(id);
        }
    }
}
