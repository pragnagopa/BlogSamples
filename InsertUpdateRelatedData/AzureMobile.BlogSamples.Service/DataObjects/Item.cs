namespace AzureMobile.BlogSamples.DataObjects
{
    public class Item
    {
        public string ItemName { get; set; }
        public string Id { get; set; }
        public string TodoItemId { get; set; }
        public virtual TodoItem TodoItem { get; set; }
    }
}