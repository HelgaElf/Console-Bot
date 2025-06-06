using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
   public interface IUserRepository
    {
   public Task<User> GetUser(Guid userId, CancellationToken ct);
   public Task<User> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
   public Task Add(User user, CancellationToken ct);

    }
}
