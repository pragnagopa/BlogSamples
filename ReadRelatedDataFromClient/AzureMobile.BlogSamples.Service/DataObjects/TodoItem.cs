using Microsoft.WindowsAzure.Mobile.Service;
using System.Collections.Generic;

namespace AzureMobile.BlogSamples.DataObjects
{
    public class TodoItem : EntityData
    {
        public TodoItem()
        {
            Items = new List<Item>();
        }
        public string Text { get; set; }
        public bool Complete { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}