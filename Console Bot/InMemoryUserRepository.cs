using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    public class InMemoryUserRepository : IUserRepository
    {
        public static List <User> UserList = new List<User>();

        public async Task<User> GetUser(Guid userId, CancellationToken ct)
        {
            foreach (var user in UserList)
            {
                if (user.UserId == userId)
                {
                    return await Task.FromResult(user);
                }
            }
            return await Task.FromResult<User>(null);
        }
        public async Task<User> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            foreach (var user in UserList)
            {
                if (telegramUserId == user.TelegramUserId)
                {
                    return await Task.FromResult(user);
                }
            }
            return await Task.FromResult<User>(null);
        }
        public async Task Add(User user, CancellationToken ct)
        {
            if(user == null)
                throw new ArgumentNullException("user");
            if (UserList.Contains(user))
                throw new Exception("User already exists");
            UserList.Add(user);   
        }
        
    }


}
