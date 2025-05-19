using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
    class InMemoryUserRepository : IUserRepository
    {
        public static List <User> UserList = new List<User>();

        private readonly IUserRepository _userRepository;
        public InMemoryUserRepository (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? GetUser(Guid userId)
        {
                User _user = null;
            foreach (var user in UserList)
            {
                if (user.UserId == userId)
                {
                    _user = user;
                    break;
                }
            }
            return _user;
        }
        public User? GetUserByTelegramUserId(long telegramUserId)
        {
                User _user = null;
            foreach (var user in UserList)
            {
                if (telegramUserId == user.TelegramUserId)
                {
                    _user = user;
                    break;
                }
            }    
            return _user;            
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
