using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace Console_Bot
{
    public class UserService : IUserService
    {
        public User RegisterUser(long telegramUserId, string telegramUserName)
        {
            return new User { TelegramUserId = telegramUserId, TelegramUserName = telegramUserName };
        }

       public  User? GetUser(long telegramUserId)
        {
            if (telegramUserId != null)
            {
                return new User { TelegramUserId = telegramUserId, TelegramUserName = "test" };
            }
            else return null;
        }
    }
}
