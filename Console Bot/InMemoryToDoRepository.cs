using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> Tasks = new List<ToDoItem>();
        //описываем методы класса
        public async Task <IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            var allTasks = new List<ToDoItem>();
            foreach (var task in Tasks)
            {
                if (task.User.UserId == userId)
                {
                    allTasks.Add(task);
                }
            }
            if (allTasks.Count == 0)
            {
                throw new Exception("Нет задач пользователя");
            }

            return await Task.FromResult(allTasks);
        }
        public Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
            {
            var result = Tasks
               .Where(item => item.User.UserId == userId)  
               .Where(predicate)                     
               .ToList()                             
               .AsReadOnly();  
            return Task.FromResult<IReadOnlyList<ToDoItem>>(result);
        }
            
        
        //Возвращает ToDoItem для UserId со статусом Active
       public async Task <IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            var activeTasks = new List<ToDoItem>();
            foreach (var task in Tasks)
            {
                if (task.State == ToDoItem.ToDoItemState.Active && task.User.UserId == userId)
                {
                    activeTasks.Add(task);
                }
            }
            return await Task.FromResult(activeTasks);
        }
       public async Task <ToDoItem?> Get(Guid id, CancellationToken ct)
        {
            ToDoItem ? getTask = null;
            foreach (var task in Tasks)
            {
                if (task.Id == id)
                {
                   getTask = task;
                }
            }
            return await Task.FromResult(getTask);
        }
        public async Task Add(ToDoItem item, CancellationToken ct)
        {
            await Task.Run(() => Tasks.Add(item), ct);
        }
       public void Update(ToDoItem item)
        {
            var index = Tasks.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                throw new ArgumentException("Задача не найдена");
            Tasks[index] = item;
        }
        public Task Delete(Guid id, CancellationToken ct)
        {
            foreach (var task in Tasks)
            {
                if (task.Id == id)
                {
                    Tasks.Remove(task);
                    break;
                }
            }
            throw new ArgumentException("Такой задачи нет");
        }
        //Проверяет есть ли задача с таким именем у пользователя
        public async Task <bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            foreach (var task in Tasks)
            {
                if (task.User.UserId == userId && task.Name == name)
                {
                    return await Task.FromResult(true);
                }
            }

            return await Task.FromResult(false);
        }
        //Возвращает количество активных задач у пользователя
        public async Task <int> CountActive(Guid userId, CancellationToken ct)
        {
            int count = 0;
            foreach (var task in Tasks)
            {
                if (task.State == ToDoItem.ToDoItemState.Active && task.User.UserId == userId)
                {
                    count++;
                }
            }
            return await Task.FromResult(count);
        }
        
    }
}
