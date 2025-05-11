using Otus.ToDoList.ConsoleBot.Types;
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
        //описываем методы класса
        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            var allTasks = new List<ToDoItem>();
            foreach (var task in Program.Tasks)
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

            return allTasks;
        }
        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
            {
            return Program.Tasks
               .Where(item => item.User.UserId == userId)  
               .Where(predicate)                     
               .ToList()                             
               .AsReadOnly();                         
            }
            
        
        //Возвращает ToDoItem для UserId со статусом Active
       public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            var activeTasks = new List<ToDoItem>();
            foreach (var task in Program.Tasks)
            {
                if (task.State == ToDoItem.ToDoItemState.Active && task.User.UserId == userId)
                {
                    activeTasks.Add(task);
                }
            }
            if (activeTasks.Count == 0)
            {
                throw new Exception("Нет активных задач");
            }

            return activeTasks;
        }
       public ToDoItem? Get(Guid id)
        {
            ToDoItem getTask =  null;
            foreach (var task in Program.Tasks)
            {
                if (task.Id == id)
                {
                   getTask = task;
                }
            }
            return getTask;
        }
        public void Add(ToDoItem item)
        {
            Program.Tasks.Add(item);
        }
       public void Update(ToDoItem item)
        {
            var index = Program.Tasks.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                throw new ArgumentException("Задача не найдена");

            Program.Tasks[index] = item;
        }
        public void Delete(Guid id)
        {
            foreach (var task in Program.Tasks)
            {
                if (task.Id == id)
                {
                    Program.Tasks.Remove(task);
                    break;
                }
            }
            throw new ArgumentException("Такой задачи нет");
        }
        //Проверяет есть ли задача с таким именем у пользователя
        public bool ExistsByName(Guid userId, string name)
        {
            foreach (var task in Program.Tasks)
            {
                if (task.User.UserId == userId && task.Name == name)
                {
                    return true;
                }
            }

            return false;
        }
        //Возвращает количество активных задач у пользователя
        public int CountActive(Guid userId)
        {
            int count = 0;
            foreach (var task in Program.Tasks)
            {
                if (task.State == ToDoItem.ToDoItemState.Active && task.User.UserId == userId)
                {
                    count++;
                }
            }
            return count;
        }
        
    }
}
