using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
   public interface IUserRepository
    {
   public User? GetUser(Guid userId);
   public User? GetUserByTelegramUserId(long telegramUserId);
   public void Add(User user);

    }
}
