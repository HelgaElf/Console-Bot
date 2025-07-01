using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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



        static async Task Main(string[] args)
        {
            IUserRepository userRepository = new InMemoryUserRepository();
            IUserService userService = new UserService(userRepository);
            IToDoRepository toDoRepository = new InMemoryToDoRepository();
            IToDoService toDoService = new ToDoService(toDoRepository);
            IToDoReportService toDoReportService = new ToDoReportService();

            var commands = new List<BotCommand>
            {
                 new() { Command = "/start", Description = "Запустить бота" },
                 new() { Command = "/addtask", Description = "Добавить новую задачу" },
                 new() { Command = "/showalltasks", Description = "Показать все задачи" },
                 new() { Command = "/showtasks", Description = "Показать завершённые задачи" },
                 new() { Command = "/addtask", Description = "Добавить новую задачу" },
                 new() { Command = "/removetask", Description = "Удалить задачу" },
                 new() { Command = "/completetask", Description = "Отметить задачу как выполненную" },
                 new() { Command = "/find", Description = "Найти задачу" },
                 new() { Command = "/info", Description = "Информация о боте" },
                 new() { Command = "/help", Description = "Помощь" },
                 new() { Command = "/report", Description = "Показать статистику" }
            };
            
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

            string ?token = Environment.GetEnvironmentVariable("TelegramBotToken", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Bot Token is not found. Please, set TelegramBotToken environment variable.");
                return;
            }
            var botClient = new TelegramBotClient(token);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message],
                DropPendingUpdates = true
            };

            
            var me = await botClient.GetMe();
            Console.WriteLine($"{me.FirstName} запущен!");
            

            var ct = new CancellationTokenSource();
            CancellationToken cancellationToken = ct.Token;

            await botClient.SetMyCommands(
                commands: commands,
                cancellationToken: cancellationToken
                );
            try
            {
                botClient.StartReceiving(handler, receiverOptions,cancellationToken);
                Console.WriteLine("Нажмите клавишу 'A' для выхода");
                while (!ct.IsCancellationRequested)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.A)
                    {
                        Console.WriteLine("\nЗавершение работы...");
                        ct.Cancel();
                        break;
                    }
                    else
                    {
                        var bot = await botClient.GetMe();
                        Console.WriteLine($"\nИнформация о боте:\n" +
                                        $"ID: {bot.Id}\n" +
                                        $"Username: @{bot.Username}\n" +
                                        $"Имя: {bot.FirstName}");
                    }
                }
                await Task.Delay(-1);
            }

            catch (Exception ex)
            {
                _ = handler.HandleErrorAsync(botClient, ex, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, cancellationToken);
                
            }
            finally
            {
                handler.OnHandleUpdateStarted -= startedHandler;
                handler.OnHandleUpdateCompleted -= completedHandler;
            }
           
        }
    }
    
}
