using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureMobile.BlogSamples.DataObjects
{
    public class Item : EntityData
    {
        public string ItemName { get; set; }
        public string TodoItemId { get; set; }
        public virtual TodoItem TodoItem { get; set; }
    }
}