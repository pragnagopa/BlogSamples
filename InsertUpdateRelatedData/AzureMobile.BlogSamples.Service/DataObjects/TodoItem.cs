using System.Collections.Generic;

namespace AzureMobile.BlogSamples.DataObjects
{
    public class TodoItem
    {
        public TodoItem()
        {
            this.Items = new List<Item>();
        }
        public string Text { get; set; }
        public string Id { get; set; }
        public bool Complete { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}