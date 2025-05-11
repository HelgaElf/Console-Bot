using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    public class ToDoReportService : IToDoReportService
    {
      public  (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            int totalCount = 0;
            int activeCount = 0;
            int completeCount = 0;
            var tasks = new List<ToDoItem>();
            foreach (var item in tasks)
            {
                if(item.User.UserId == userId)
                {
                    if(item.State == ToDoItem.ToDoItemState.Active)
                    {
                        activeCount++;
                    }

                    if (item.State == ToDoItem.ToDoItemState.Completed)
                    {
                        completeCount++;
                    }

                    totalCount++;
                }
            
            }

            return (
                total: totalCount,
                completed: completeCount,
                active: activeCount,
                generatedAt: DateTime.UtcNow
                    );
        }
    }
}
