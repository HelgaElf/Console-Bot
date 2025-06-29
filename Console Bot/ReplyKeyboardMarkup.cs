using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Console_Bot
{
    class BotKeyboard
    {
        //  клавиатура с одной кнопкой "СТАРТ"
        public static ReplyKeyboardMarkup StartKeyboard => new(new[]
        {
        new[] { new KeyboardButton("/start") }
    })
        {
            ResizeKeyboard = true
        };

        // 2. Основное меню
        public static ReplyKeyboardMarkup MainMenu => new(new[]
        {
        new[] { new KeyboardButton("/showalltasks") }, 
        new[] { new KeyboardButton("/showtasks") },
        new[] { new KeyboardButton("/report") }
    })
        {
            ResizeKeyboard = true
        };

    }


}

