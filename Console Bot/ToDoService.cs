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

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
           var activeTasks = _inMemoryToDo.GetActiveByUserId(userId);
            return activeTasks;
        }  
        public ToDoItem Add(User user, string name)
        {
            if (name == null) throw new ArgumentNullException("Введите название задачи");
           var exist = _inMemoryToDo.ExistsByName(user.UserId, name);
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
           _inMemoryToDo.Add(item);
            return item;
        }
       public void MarkCompleted(Guid id)
        {
            var task = _inMemoryToDo.Get(id);

            if (task != null)
            {
                task.State = ToDoItem.ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;
                _inMemoryToDo.Update(task);
            }
            else throw new ArgumentException("Не существует задачи с ID - " + id);
          
       }
        public void Delete(Guid id) {

           _inMemoryToDo.Delete(id);
            
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            var allTasks = _inMemoryToDo.GetAllByUserId(userId);
            return allTasks;
        }
        public int CountActive(Guid userId)
        {
            int activeCount =_inMemoryToDo.CountActive(userId);
            return activeCount;
        }

       public IReadOnlyList<ToDoItem> Find(User user, string namePrefix)
        {
            return _inMemoryToDo.Find(user.UserId,
        item => item.Name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}



