using AutoMapper;
using AzureMobile.BlogSamples.DataObjects;
using AzureMobile.BlogSamples.Models;
using AzureMobile.BlogSamples.Utilities;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;

namespace AzureMobile.BlogSamples.Controllers
{
    public class TodoItemController : TableController<TodoItemDTO>
    {
        private ServiceDBContext context;
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            this.context = new ServiceDBContext();
            DomainManager = new SimpleMappedEntityDomainManager<TodoItemDTO, TodoItem>
                                (context, Request, Services, todoItem => todoItem.Id);
        }

        public IQueryable<TodoItemDTO> GetAllTodoItems()
        {
            return Query();
        }

        public SingleResult<TodoItemDTO> GetTodoItem(string id)
        {
            return Lookup(id);
        }

        public async Task<TodoItemDTO> PatchTodoItem(string id,
            Delta<TodoItemDTO> patch)
        {
            //Read TodoItem to update from database so that EntityFramework updates
            //existing entry
            TodoItem currentTodoItem = this.context.TodoItems.Include("Items")
                                    .First(j => (j.Id == id));

            //Convert database type to client type in order to update only properties
            //in the incoming patch request
            TodoItemDTO todoItemDTOUpdated = Mapper.Map<TodoItem, TodoItemDTO>
                                            (currentTodoItem);

            //Apply changes updates from the the incoming request
            patch.Patch(todoItemDTOUpdated);

            //Convert back to database type
            Mapper.Map<TodoItemDTO, TodoItem>(todoItemDTOUpdated, currentTodoItem);

            //Apply updates to related items
            if (todoItemDTOUpdated.Items != null)
            {
                currentTodoItem.Items = new List<Item>();
                foreach (ItemDTO currentItemDTO in todoItemDTOUpdated.Items)
                {
                    Item existingItem = this.context.Items
                                .FirstOrDefault(j => (j.Id == currentItemDTO.Id));
                    if (existingItem != null)
                    {
                        //Convert client type to database type
                        existingItem = Mapper.Map<ItemDTO, Item>(currentItemDTO,
                                existingItem);
                        existingItem.TodoItem = currentTodoItem;
                        //Attach to parent entity.
                        currentTodoItem.Items.Add(existingItem);
                        this.context.Entry(existingItem).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            await this.context.SaveChangesAsync();

            //Convert to client type before returning the result
            var result = Mapper.Map<TodoItem, TodoItemDTO>(currentTodoItem);
            return result;
        }

        public async Task<IHttpActionResult> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            //Entity Framework inserts new TodoItem and any related entities
            //sent in the incoming request
            TodoItemDTO current = await InsertAsync(todoItemDTO);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteTodoItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}