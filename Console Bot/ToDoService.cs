using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _inMemoryToDo;
        public ToDoService(IToDoRepository toDoRepository)
        {
            _inMemoryToDo = toDoRepository;
        }

        public async Task <IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            var activeTasks = await _inMemoryToDo.GetActiveByUserId(userId, ct);
            return await Task.FromResult(activeTasks);
        }  
        public async Task <ToDoItem> Add(User user, string name, CancellationToken ct)
        {
            if (name == null) throw new ArgumentNullException("Введите название задачи");
           var exist = await _inMemoryToDo.ExistsByName(user.UserId, name, ct);
            if (exist)
            {
                throw new ArgumentException("Задача с таким именем уже существует");
            }

            var item = new ToDoItem
            { 
                Id = Guid.NewGuid(),
                User = user,
                Name = name,  
                CreatedAt = DateTime.Now,
                State = ToDoItem.ToDoItemState.Active
            };
          await _inMemoryToDo.Add(item, ct);
            return await Task.FromResult(item);
        }
       public async Task MarkCompleted(Guid id, CancellationToken ct)
        {
            var task = await _inMemoryToDo.Get(id, ct);

            if (task != null)
            {
                task.State = ToDoItem.ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;
                await _inMemoryToDo.Update(task, ct);
            }
            else throw new ArgumentException("Не существует задачи с ID - " + id);
          
       }
        public async Task Delete(Guid id, CancellationToken ct) {

           await _inMemoryToDo.Delete(id, ct);
            
        }

        public async Task <IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            var allTasks = await _inMemoryToDo.GetAllByUserId(userId, ct);
            return await Task.FromResult(allTasks);
        }
        public async Task <int> CountActive(Guid userId, CancellationToken ct)
        {
            int activeCount =await _inMemoryToDo.CountActive(userId, ct);
            return await Task.FromResult(activeCount);
        }

       public async Task<IReadOnlyList<ToDoItem>> Find(User user, string namePrefix, CancellationToken ct)
        {
            return await _inMemoryToDo.Find(user.UserId,
        item => item.Name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase), ct);
        }
    }
}



