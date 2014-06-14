using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using AzureMobile.BlogSamples.DataObjects;
using AzureMobile.BlogSamples.Models;

namespace AzureMobile.BlogSamples
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            Database.SetInitializer(new ServiceDBInitializer());
        }
    }

    public class ServiceDBInitializer : DropCreateDatabaseIfModelChanges<ServiceDBContext>
    {
        protected override void Seed(ServiceDBContext context)
        {
            List<Item> items = new List<Item>
            {
                new Item { Id = "1", ItemName = "Milk" },
                new Item { Id = "2", ItemName = "Eggs" }
            };

            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = "1", Text = "Grocery", Complete = false, Items=items }
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}

