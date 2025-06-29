using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Console_Bot
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List <User> UserList = new List<User>();

        public async Task<User?> GetUser(Guid userId, CancellationToken ct)
        {
            foreach (var user in UserList)
            {
                if (user.UserId == userId)
                {
                    return await Task.FromResult(user);
                }
            }
            return null;
        }
        public async Task<User?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            foreach (var user in UserList)
            {
                if (telegramUserId == user.TelegramUserId)
                {
                    return await Task.FromResult(user);
                }
            }
            return null;
        }
        public void Add(User user)
        {
            if(user == null)
                throw new ArgumentNullException("user");
            if (UserList.Contains(user))
                throw new Exception("User already exists");
            UserList.Add(user);
        }
        
    }


}
