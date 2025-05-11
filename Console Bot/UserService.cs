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
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User RegisterUser(long telegramUserId, string telegramUserName)
        {
           User newUser = new User
            {
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                UserId = Guid.NewGuid()
            };

            _userRepository.Add(newUser);

            return newUser;
        }

       public User? GetUser(Guid UserId)
        {
            var user = _userRepository.GetUser(UserId);
            return user;
        }

       public User? GetUserByTelegramUserID(long telegramUserId)
        {
            var user = _userRepository.GetUserByTelegramUserId(telegramUserId);
            return user;
        }


    }
}
