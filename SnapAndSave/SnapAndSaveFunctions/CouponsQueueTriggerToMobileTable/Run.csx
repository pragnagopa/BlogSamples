using System;
using Microsoft.Azure.NotificationHubs;

public static void Run(Coupon myQueueItem, TraceWriter log, Coupon inputRecord, out Notification notification)
{
    log.Info($"C# Queue trigger function started");
    log.Info($"Dequeued item: {myQueueItem.Id}");
    // outputRecord.Add(myQueueItem);
    inputRecord.Description = myQueueItem.Description;
    inputRecord.Expiry = myQueueItem.Expiry;
    log.Info($"Write item to Mobile Table: {myQueueItem.Id}");
    
    string payload = "{\"data\": {\"message\":\"" + inputRecord.Description + "\"}}";
    log.Info($"Payload: {payload}");
    notification = new GcmNotification(payload);

    
}

public class Coupon
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTime Expiry { get; set; }
}

