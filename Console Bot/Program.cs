using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
namespace Console_Bot
{
    internal class Program
    {
        public static bool EchoCommand = false;
        public static string userName = string.Empty;
        public static List<string> Tasks = new List<string>();
        public static int taskCountLimit;
        public static int taskLengthLimit;
        public static int min = 0;
        public static int max = 100;
        static void Main(string[] args)
        {
            var handler = new UpdateHandler(userName, Tasks, taskCountLimit, taskLengthLimit);

            Console.WriteLine("Добро пожаловать! Выберите команду: /start (для начала работы), /help (если требуется помощь), /info (о боте), /exit (для выхода)");
            while (true)
            {
                try
                {
                    handler.HandlerUpdateAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    /* Console.WriteLine("Ошибка: " + ex.Message);
                     Console.WriteLine(ex.StackTrace);
                     Console.WriteLine(ex.InnerException);
                     Console.WriteLine("Выберите команду: /start (для начала работы), /help (если требуется помощь), /info (о боте), /exit (для выхода)");
                */

                    var userService = new UserService(); // Создаём сервис
                    var updateHandler = new UpdateHandler(userService);
                }
            }
        }
    }
}
