using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
   public interface IUserService
    {
            User RegisterUser(long telegramUserId,  string telegramUserName);
            User? GetUser(Guid UserId);
            User? GetUserByTelegramUserID(long telegramUserId);
    }
}
