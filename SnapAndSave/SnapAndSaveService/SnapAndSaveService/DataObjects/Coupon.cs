using Microsoft.Azure.Mobile.Server;
using System;

namespace SnapAndSaveService.DataObjects
{
    public class Coupon : EntityData
    {
        public string Description { get; set; }
        public DateTime Expiry { get; set; }
    }
}