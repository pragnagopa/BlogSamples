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

            TodoItemDTO updatedpatchEntity = patch.GetEntity();
            ICollection<ItemDTO> updatedItems;

            //Check if incoming request contains Items
            bool requestContainsRelatedEntities = patch.GetChangedPropertyNames().Contains("Items");

            if (requestContainsRelatedEntities)
            {
                //Remove related entities from the database. Comment following for loop if you do not
                //want to delete related entities from the database
                for (int i = 0; i < currentTodoItem.Items.Count && updatedpatchEntity.Items!=null; i++)
                {
                    ItemDTO itemDTO = updatedpatchEntity.Items.FirstOrDefault(j =>
                                    (j.Id == currentTodoItem.Items.ElementAt(i).Id));
                    if (itemDTO == null)
                    {
                        this.context.Items.Remove(currentTodoItem.Items.ElementAt(i));
                    }
                }

                //If request contains Items get the updated list from the patch
                Mapper.Map<TodoItemDTO, TodoItem>(updatedpatchEntity, currentTodoItem);
                updatedItems = updatedpatchEntity.Items;
            }
            else
            {
                //If request doest not have Items, then retain the original association
                TodoItemDTO todoItemDTOUpdated = Mapper.Map<TodoItem, TodoItemDTO>
                                                (currentTodoItem);
                patch.Patch(todoItemDTOUpdated);
                Mapper.Map<TodoItemDTO, TodoItem>(todoItemDTOUpdated, currentTodoItem);
                updatedItems = todoItemDTOUpdated.Items;
            }

            if (updatedItems != null)
            {
                //Update related Items
                currentTodoItem.Items = new List<Item>();
                foreach (ItemDTO currentItemDTO in updatedItems)
                {
                    //Look up existing entry in database
                    Item existingItem = this.context.Items
                                .FirstOrDefault(j => (j.Id == currentItemDTO.Id));
                    //Convert client type to database type
                    existingItem = Mapper.Map<ItemDTO, Item>(currentItemDTO,
                            existingItem);
                    existingItem.TodoItem = currentTodoItem;
                    //Attach to parent entity.
                    currentTodoItem.Items.Add(existingItem);
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