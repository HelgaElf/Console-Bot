using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using static Console_Bot.ToDoItem;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Console_Bot
{
    public delegate void MessageEventHandler(string message);
    class UpdateHandler : IUpdateHandler
    {
        public event MessageEventHandler ? OnHandleUpdateStarted;
        public event MessageEventHandler? OnHandleUpdateCompleted;
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
            string messageText = update.Message.Text;
                OnHandleUpdateStarted?.Invoke(messageText);
                string input = update.Message.Text;
                long userID = update.Message.From.Id;
                var user =  await _userService.GetUserByTelegramUserID(userID,cancellationToken);
            

                string command = input?.ToLower().Split(' ')[0];

                if (user == null && command != "/help" && command != "/info" && command != "/start")
                {
                _ = await botClient.SendMessage(update.Message.Chat,
               text: "Для начала работы нажмите СТАРТ",
               replyMarkup: BotKeyboard.StartKeyboard,
               cancellationToken: cancellationToken);

            }

           switch (command)
           {
                    case "/start":
                      await Start(botClient, update, cancellationToken);
                    break;
                    case "/help":
                        await Help(botClient, update, cancellationToken);
                        break;
                    case "/info":
                        await Info(botClient, update, cancellationToken);
                        break;
                    case "/addtask":
                        await AddTask(botClient, update, input, cancellationToken);
                        break;
                    case "/showtasks":
                        await Showtasks(botClient, update, cancellationToken);
                        break;
                    case "/removetask":
                        await RemoveTasks(botClient, update, input, cancellationToken);
                        break;
                    case "/exit":
                        await Exit(botClient, update, cancellationToken);
                        break;
                    case "/completetask":
                        await CompleteTask(botClient, update, input, cancellationToken);
                        break;
                    case "/showalltasks":
                        await ShowAllTasks(botClient, update, cancellationToken);
                        break;
                    case "/report":
                        await Report(botClient, update, cancellationToken);
                        break;
                    case "/find":
                        await Find(botClient, update, input, cancellationToken);
                        break;
                    default:
                    await botClient.SendMessage(update.Message.Chat, $"Введите одну из предложенных команд: ");
                    throw new ArgumentException("Введите одну из предложенных команд!");

           }
                OnHandleUpdateCompleted?.Invoke(messageText);
        }
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"HandleError: {exception}, source: {source}");
            
            return Task.CompletedTask;
        }

        private async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
                string userName = update.Message.From.Username ?? "User";
                long userID = update.Message.From.Id;

                User newUser = await _userService.RegisterUser(userID, userName, cancellationToken);

                await botClient.SendMessage(update.Message.Chat, $"Пользователь зарегистрирован, '{userName}'");
            _ = await botClient.SendMessage(update.Message.Chat,
                text: "Выберите действие: ",
                replyMarkup: BotKeyboard.MainMenu,
                cancellationToken: cancellationToken);
        }

        static async Task Help(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(update.Message.Chat, $" чтобы начать работу, введите команду /start "
                 + "Чтобы добавить новую задачу введите /addtask \"Имя задачи\", чтобы посмотреть список задач, введите /showtask, чтобы удалить задачу, введите /removetask");
        }

        static async Task Info(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(update.Message.Chat, $"Версия 0.1.9. Создано 24.02.2025");
        }

        static async Task Exit(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(update.Message.Chat, $"До новых встреч!");
            Environment.Exit(0);
        }

        async Task AddTask(ITelegramBotClient botClient, Update update, string newTask, CancellationToken cancellationToken)
        {
            string userName = update.Message.From.Username ?? "User";
            long userID = update.Message.From.Id;
            var user = await _userService.GetUserByTelegramUserID(userID, cancellationToken);

            string taskText = newTask.Substring(8);
   
            ToDoItem task = await _toDoService.Add(user, taskText, cancellationToken);
            await botClient.SendMessage(update.Message.Chat,
                $"Пользователь '{userName}' задача <code>{task.Id}</code> добавлена '{task.Name}'",
                parseMode: ParseMode.Html
                );
        }

        async Task Showtasks(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
          
            long telegramUserID = update.Message.From.Id;
            var user = await _userService.GetUserByTelegramUserID(telegramUserID, cancellationToken);

            var activeTask = await _toDoService.GetActiveByUserId(user.UserId, cancellationToken);
                    

            if (activeTask.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Нет активных задач!");
            }
            else {
                foreach (var task in activeTask)
                {
                    await botClient.SendMessage(update.Message.Chat,
                        $"- '{task.Name}' - {task.CreatedAt} - <code>{task.Id}</code>",
                        parseMode: ParseMode.Html);
                }
            }
        }
        async Task RemoveTasks(ITelegramBotClient botClient, Update update, string input, CancellationToken cancellationToken)
        {

                long telegramUserID = update.Message.From.Id;
                var user = await _userService.GetUserByTelegramUserID(telegramUserID, cancellationToken);
                var allTasks = await _toDoService.GetAllByUserId(user.UserId, cancellationToken);
                if (allTasks.Count != 0)
                {
                    string taskGuid = input.Substring(12);
                    if (Guid.TryParse(taskGuid, out var id))
                    {
                        await _toDoService.Delete(id, cancellationToken);
                        await botClient.SendMessage(update.Message.Chat, $"Задача '{id}' удалена '{update.Message.Text}");
                    }
                    else
                    {
                        throw new ArgumentException("Невалидный id задачи");
                    }
                }
                else
                {
                    throw new ArgumentException("Список задач пуст!");
                }
        }
        static public void ValidateString(string? str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Строка не может быть пустой!");
            }
        }
        async Task CompleteTask(ITelegramBotClient botClient, Update update, string input, CancellationToken cancellationToken)
        {
            string taskGuid = input.Substring(14);
            if (Guid.TryParse(taskGuid, out var id))
            {
                await _toDoService.MarkCompleted(id, cancellationToken);
                await botClient.SendMessage(update.Message.Chat, $"Задача '{id}' отмечена как выполненная '{update.Message.Text}");
            }
        }

        async Task ShowAllTasks(ITelegramBotClient botClient, Update update,  CancellationToken cancellationToken)
        {
            long telegramUserID = update.Message.From.Id;
            var user = await _userService.GetUserByTelegramUserID(telegramUserID, cancellationToken);
            var allTasks = await _toDoService.GetAllByUserId(user.UserId, cancellationToken);
            foreach (var task in allTasks)
            {
                await botClient.SendMessage(update.Message.Chat, 
                    $" ({task.State}) {task.Name} - {task.CreatedAt} - <code>{task.Id}</code>", 
                    parseMode: ParseMode.Html);
            }
        }
        async Task Report(ITelegramBotClient botClient, Update update,  CancellationToken cancellationToken)
        {
                long telegramUserID = update.Message.From.Id;
                User user = await _userService.GetUserByTelegramUserID(telegramUserID, cancellationToken);

                if (user != null)
                {

                    var report = await _toDoReportService.GetUserStats(user.UserId, cancellationToken);
                    await botClient.SendMessage(update.Message.Chat, $"Статистика по задачам на  {report.generatedAt:dd.MM.yyyy HH:mm:ss}. Всего {report.total}; " +
                        $"Завершённых: {report.completed}; Активных: {report.active}.");
                }
        }

        async Task Find(ITelegramBotClient botClient, Update update, string input, CancellationToken cancellationToken)
        {
                long telegramUserID = update.Message.From.Id;
                User user = await _userService.GetUserByTelegramUserID(telegramUserID, cancellationToken);

                string namePrefix = input.Substring(6);
                ValidateString(namePrefix);
                var tasks = await _toDoService.Find(user, namePrefix, cancellationToken);
                if (tasks == null)
                {
                    await botClient.SendMessage(update.Message.Chat, $"Задачи не найдены!");
                }
                else
                {
                    foreach (var task in tasks)
                    {
                        await botClient.SendMessage(update.Message.Chat, $"- '{task.Name}' - {task.CreatedAt} - {task.Id}");
                    }
                }
        }

        //классы 
        class DuplicateTaskException : Exception
        {
            public DuplicateTaskException(string task)
                : base($"Задача '{task}' уже существует")
            {
            }
        }
        class TaskCountLimitException : Exception
        {
            public TaskCountLimitException(int taskCountLimit)
                : base($"Превышено максимально допустимое количество задач, равное {taskCountLimit}. ")
            {
            }
        }
    }

    class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
            : base($"Длина задачи '{taskLength}' превышает максимально допустимое значение {taskLengthLimit}")
        {
        }
    }
               
    }

