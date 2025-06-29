using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Console_Bot
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
           User newUser = new User
            {
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUserName,
                UserId = Guid.NewGuid(),
                RegisteredAt = DateTime.Now,
            };

           _userRepository.Add(newUser);
            return await Task.FromResult(newUser);

        }

       public async Task <User> GetUser(Guid UserId, CancellationToken ct)
        {
            var user = await _userRepository.GetUser(UserId, ct);
            return user;
        }

       public async Task <User> GetUserByTelegramUserID(long telegramUserId, CancellationToken ct)
        {
            var user = await _userRepository.GetUserByTelegramUserId(telegramUserId, ct);
            return user;
        }


    }
}
