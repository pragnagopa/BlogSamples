#r "System.Linq.Expressions"
#r "System.Runtime"
#r "System.Threading.Tasks"
#r "System.Collections"

using System;
using System.Collections;
using Microsoft.Azure.NotificationHubs;
using Microsoft.WindowsAzure.MobileServices;


public static async Task Run(TimerInfo myTimer, TraceWriter log, IMobileServiceTable<Coupon> table, IAsyncCollector<Notification> notification)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");    
    var processedItems = await table.CreateQuery()
                .Where(coupon => coupon.Expiry < DateTime.Now.AddYears(1))
                .ToListAsync();
                
    if (processedItems != null)
    {
        foreach (var item in processedItems)
        {
            string payload = "{\"data\": {\"message\":\"" + item.Description + " coupon expiring soon!\"}}";
            log.Info($"Payload: {payload}");
            await notification.AddAsync(new GcmNotification(payload));
        }
    }
}

public class Coupon
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTime Expiry { get; set; }
}
