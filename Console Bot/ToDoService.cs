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
        public ToDoItem Add(User user, string name)
        {
            if (name == null) throw new ArgumentNullException("Введите название задачи");
            var item = new ToDoItem { 
                Id = Guid.NewGuid(),
                User = user,
                Name = name,  
                CreatedAt = DateTime.Now,
                State = ToDoItem.ToDoItemState.Active
            };
            Program.Tasks.Add(item);
            return item;
        }
       public void MarkCompleted(Guid id)
        {
            var index = GetIndex(id);
            Program.Tasks[index].State = ToDoItem.ToDoItemState.Completed;
            Program.Tasks[index].StateChangedAt = DateTime.Now;

        }
        public void Delete(Guid id) {
            var index = GetIndex(id);
            Program.Tasks.RemoveAt(index);
        }

        private int GetIndex(Guid id)
        {
            bool findIndex = false;
            int index = 0;
            foreach (var task in Program.Tasks)
            {
                if (task.Id == id)
                {
                    findIndex = true;
                    break;
                }
                index++;
            }
            if (findIndex == false)
            { throw new ArgumentException("Такой id не существует"); }
            
            return index;
        }
    }


}
