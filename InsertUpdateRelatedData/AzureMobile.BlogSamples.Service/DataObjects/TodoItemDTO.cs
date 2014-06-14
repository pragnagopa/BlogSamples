using Microsoft.WindowsAzure.Mobile.Service;
using System.Collections.Generic;

namespace AzureMobile.BlogSamples.DataObjects
{
    public class TodoItemDTO : EntityData
    {
        public string Text { get; set; }
        public bool Complete { get; set; }
        public virtual ICollection<ItemDTO> Items { get; set; }
    }
}