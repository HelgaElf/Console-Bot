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
       
        public static int taskCountLimit;
        public static int taskLengthLimit;
        public static int min = 0;
        public static int max = 100;
        public static long isRegisteredUser;
        public static bool active = false;

        static void Main(string[] args)
        {
            IUserRepository userRepository = new InMemoryUserRepository();
            IUserService userService = new UserService(userRepository);
            IToDoRepository toDoRepository = new InMemoryToDoRepository();
            IToDoService toDoService = new ToDoService(toDoRepository);
            IToDoReportService toDoReportService = new ToDoReportService();

            MessageEventHandler startedHandler = message =>
            {
                try
                {
                    Console.WriteLine($"Началась обработка сообщения: '{message}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в OnHandleUpdateStarted: {ex.Message}");
                }
            };

            MessageEventHandler completedHandler = message =>
                Console.WriteLine($"Завершена обработка сообщения: '{message}'");

            var handler = new UpdateHandler(userService, toDoService, toDoReportService);
            handler.OnHandleUpdateStarted += startedHandler;
            handler.OnHandleUpdateCompleted += completedHandler;

            var botClient = new ConsoleBotClient();
            var ct = new CancellationTokenSource();
            CancellationToken token = ct.Token;
            try
            {
                botClient.StartReceiving(handler, token);
            }

            catch (Exception ex)
            {
                handler.HandleErrorAsync(botClient, ex,token);
            }
            finally
            {
                handler.OnHandleUpdateStarted -= startedHandler;
                handler.OnHandleUpdateCompleted -= completedHandler;
            }
        }
    }
    
}
