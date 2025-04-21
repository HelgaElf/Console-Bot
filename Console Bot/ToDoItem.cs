using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
   public class ToDoItem
    {
        public enum ToDoItemState
        {
            Active, 
            Completed
        }
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }
    }

        
}

