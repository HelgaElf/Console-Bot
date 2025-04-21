using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
namespace Console_Bot
{
    internal class Program
    {
        public static bool EchoCommand = false;
        public static string userName = string.Empty;
        public static List<ToDoItem> Tasks = new List<ToDoItem>();
        public static int taskCountLimit;
        public static int taskLengthLimit;
        public static int min = 0;
        public static int max = 100;
        public static long isRegisteredUser;
        public static bool active = false;

        static void Main(string[] args)
        {
            IUserService userService = new UserService();
            var handler = new UpdateHandler(userService);
            var botClient = new ConsoleBotClient();
            
            botClient.StartReceiving(handler);

            //var botClient = new ConsoleBotClient();
            // botClient.StartReceiving(handler);

            //var handler = new UpdateHandler(botClient, update);

            while(true)
            {
                string input = Console.ReadLine();
                var update = new Update { Message = new Message { Text = input, Chat = new Chat { Id = 0 } } };
                handler.HandleUpdateAsync(botClient, update);
            }

            handler.HandleUpdateAsync(botClient, new Update
            {
                Message = new Message
                {
                    Text = active ? "/start" : "/help",
                    Chat = new Chat { Id = 123 }
                }
            });
            Console.WriteLine("Добро пожаловать! Выберите команду:  /help (если требуется помощь), /info (о боте)");
            
               catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    var userService = new UserService(); // Создаём сервис
                    var updateHandler = new UpdateHandler(userService);
                }
            }
        }
    }
}
