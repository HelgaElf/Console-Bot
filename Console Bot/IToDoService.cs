using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    public interface IToDoService
    {
     	
	  IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
	  ToDoItem Add(User user, string name);
	  void MarkCompleted(Guid id);
	  void Delete(Guid id);
      IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
      int CountActive(Guid userId);

      IReadOnlyList<ToDoItem> Find(User user, string namePrefix);
    }

}

