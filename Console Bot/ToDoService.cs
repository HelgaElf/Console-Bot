using Telegram.Bot.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Console_Bot
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _inMemoryToDo;
        public ToDoService(IToDoRepository toDoRepository)
        {
            _inMemoryToDo = toDoRepository;
        }

        public Task <IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var activeTasks = _inMemoryToDo.GetActiveByUserId(userId, cancellationToken);
            return activeTasks;
        }  
        public async Task <ToDoItem> Add(User user, string name, CancellationToken cancellationToken)
        {
            name.TrimStart();
            if (name == null) throw new ArgumentNullException("Введите название задачи");
           var exist = await _inMemoryToDo.ExistsByName(user.UserId, name, cancellationToken);
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
          await _inMemoryToDo.Add(item, cancellationToken);
            return await Task.FromResult(item);
        }
       public async Task MarkCompleted(Guid id, CancellationToken cancellationToken)
        {
            var task = await _inMemoryToDo.Get(id, cancellationToken);

            if (task != null)
            {
                task.State = ToDoItem.ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;
                _inMemoryToDo.Update(task);
            }
            else throw new ArgumentException("Не существует задачи с ID - " + id);
          
       }
        public async Task Delete(Guid id, CancellationToken cancellationToken) {

           await _inMemoryToDo.Delete(id, cancellationToken);
            
        }

        public async Task <IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var allTasks = await _inMemoryToDo.GetAllByUserId(userId, cancellationToken);
            return await Task.FromResult(allTasks);
        }
        public async Task <int> CountActive(Guid userId, CancellationToken cancellationToken)
        {
            int activeCount =await _inMemoryToDo.CountActive(userId, cancellationToken);
            return await Task.FromResult(activeCount);
        }

       public async Task<IReadOnlyList<ToDoItem>> Find(User user, string namePrefix, CancellationToken cancellationToken)
        {
            return await _inMemoryToDo.Find(user.UserId,
        item => item.Name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }
    }
}



