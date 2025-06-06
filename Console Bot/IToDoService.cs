using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    public interface IToDoService
    {

      Task <IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct);
	  Task <ToDoItem> Add(User user, string name, CancellationToken ct);
	  Task MarkCompleted(Guid id, CancellationToken ct);
	  Task Delete(Guid id, CancellationToken ct);
      Task <IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct);
      Task<int> CountActive(Guid userId, CancellationToken ct);

      Task <IReadOnlyList<ToDoItem>> Find(User user, string namePrefix, CancellationToken ct);
    }

}

