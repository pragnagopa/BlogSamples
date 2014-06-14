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
            TodoItem currentTodoItem = this.context.TodoItems.Include("Items")
                                    .First(j => (j.Id == id));
            TodoItemDTO todoItemDTOUpdated = Mapper.Map<TodoItem, TodoItemDTO>
                                            (currentTodoItem);
            patch.Patch(todoItemDTOUpdated);
            Mapper.Map<TodoItemDTO, TodoItem>(todoItemDTOUpdated, currentTodoItem);

            if (todoItemDTOUpdated.Items != null)
            {
                currentTodoItem.Items = new List<Item>();
                foreach (ItemDTO currentItemDTO in todoItemDTOUpdated.Items)
                {
                    Item existingItem = this.context.Items
                                .FirstOrDefault(j => (j.Id == currentItemDTO.Id));
                    if (existingItem != null)
                    {
                        existingItem = Mapper.Map<ItemDTO, Item>(currentItemDTO,
                                existingItem);
                        existingItem.TodoItem = currentTodoItem;
                        currentTodoItem.Items.Add(existingItem);
                        this.context.Entry(existingItem).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }

            await this.context.SaveChangesAsync();

            var result = Mapper.Map<TodoItem, TodoItemDTO>(currentTodoItem);
            return result;
        }

        public async Task<IHttpActionResult> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            TodoItemDTO current = await InsertAsync(todoItemDTO);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteTodoItem(string id)
        {

            return DeleteAsync(id);
        }
    }
}