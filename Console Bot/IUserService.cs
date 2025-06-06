using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
   public interface IUserService
    {
            Task <User> RegisterUser(long telegramUserId,  string telegramUserName, CancellationToken ct);
            Task <User> GetUser(Guid UserId, CancellationToken ct);
            Task <User> GetUserByTelegramUserID(long telegramUserId, CancellationToken ct);
    }
}
