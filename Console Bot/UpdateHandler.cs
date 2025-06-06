using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using static Console_Bot.ToDoItem;

namespace Console_Bot
{
    public delegate void MessageEventHandler(string message);
    class UpdateHandler : IUpdateHandler
    {
        public event MessageEventHandler OnHandleUpdateStarted;
        public event MessageEventHandler OnHandleUpdateCompleted;
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                string messageText = update.Message.Text;
                OnHandleUpdateStarted?.Invoke(messageText);
                string input = update.Message.Text;
                string command = input?.ToLower().Split(' ')[0];

                if (!Program.active && command != "/help" && command != "/info" && command != "/start")
                {
                    await botClient.SendMessage(update.Message.Chat, $"Для начала работы введите команду /start'{update.Message.From.Username}'", ct);

                }

                switch (command)
                {
                    case "/start":
                        Start(botClient, update, ct);
                        break;
                        //case "/echo":
                        //  Echo(input);
                        break;
                    case "/help":
                        Help(botClient, update, ct);
                        break;
                    case "/info":
                        Info(botClient, update, ct);
                        break;
                    case "/addtask":
                        AddTask(botClient, update, input, ct);
                        break;
                    case "/showtasks":
                        Showtasks(botClient, update, Program.Tasks, ct);
                        break;
                    case "/removetask":
                        RemoveTasks(botClient, update, input, ct);
                        break;
                    case "/exit":
                        Exit(botClient, update, ct);
                        break;
                    case "/completetask":
                        CompleteTask(botClient, update, input, ct);
                        break;
                    case "/showalltasks":
                        ShowAllTasks(botClient, update, Program.Tasks, ct);
                        break;
                    case "/report":
                        Report(botClient, update, Program.Tasks, ct);
                        break;
                    case "/find":
                        Find(botClient, update, input, ct);
                        break;
                    default:
                        throw new ArgumentException("Введите одну из предложенных команд!");

                }
                OnHandleUpdateCompleted?.Invoke(messageText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                await HandleErrorAsync(botClient, ex, ct);
            }
        }
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }

        private async Task Start(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                string userName = update.Message.From.Username ?? "User";
                long userID = update.Message.From.Id;

                User newUser = await _userService.RegisterUser(userID, userName, ct);

                Program.active = true;
                Program.isRegisteredUser = newUser.TelegramUserId;

                await botClient.SendMessage(update.Message.Chat, $"Пользователь зарегистрирован, '{userName}'", ct);
            }
            catch (Exception ex) 
            {
             await HandleErrorAsync(botClient, ex, ct);
            }

        }

        static async Task Help(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            await botClient.SendMessage(update.Message.Chat, $" чтобы начать работу, введите команду /start "
                 + "Чтобы добавить новую задачу введите /addtask \"Имя задачи\", чтобы посмотреть список задач, введите /showtask, чтобы удалить задачу, введите /removetask", ct);
        }

        static async Task Info(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            await botClient.SendMessage(update.Message.Chat, $"Версия 0.1.5 Создано 24.02.2025", ct);
        }

        static async Task Exit(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            await botClient.SendMessage(update.Message.Chat, $"До новых встреч!", ct);
            Environment.Exit(0);
        }

        async Task AddTask(ITelegramBotClient botClient, Update update, string newTask, CancellationToken ct)
        {
            try { 
            string userName = update.Message.From.Username ?? "User";
            long userID = update.Message.From.Id;
            var user = await _userService.GetUserByTelegramUserID(userID, ct);

            string taskText = newTask.Substring(9);
            ValidateString(taskText);

                ToDoItem task = await _toDoService.Add(user, taskText, ct);
                await botClient.SendMessage(update.Message.Chat, $"Пользователь '{userName}' задача '{task.Id}' добавлена '{task.Name}'", ct);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, ct);
            }
            
        }

        static async Task Showtasks(ITelegramBotClient botClient, Update update, List<ToDoItem> tasks, CancellationToken ct)
        {
            var activeTasks = new List<ToDoItem>();
            foreach (var task in tasks)
            {
                if (task.State == ToDoItem.ToDoItemState.Active)
                {
                    activeTasks.Add(task);
                }
            }

            if (activeTasks.Count == 0)
            {
                await botClient.SendMessage(update.Message.Chat, $"Нет активных задач!", ct);
            }
            else {
                foreach (var task in tasks)
                {
                    await botClient.SendMessage(update.Message.Chat, $"- '{task.Name}' - {task.CreatedAt} - {task.Id}", ct);
                }
            }
        }
        async Task RemoveTasks(ITelegramBotClient botClient, Update update, string input, CancellationToken ct)
        {
            try
            {
                if (Program.Tasks.Count != 0)
                {
                    string taskGuid = input.Substring(12);
                    if (Guid.TryParse(taskGuid, out var id))
                    {
                        await _toDoService.Delete(id, ct);
                        await botClient.SendMessage(update.Message.Chat, $"Задача '{id}' удалена '{update.Message.Text}", ct);
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
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, ct);
            }
        }
        static public void ValidateString(string? str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Строка не может быть пустой!");
            }
        }

        async Task CompleteTask(ITelegramBotClient botClient, Update update, string input, CancellationToken ct)
        {
            string taskGuid = input.Substring(14);
            if (Guid.TryParse(taskGuid, out var id))
            {
                await _toDoService.MarkCompleted(id, ct);
                await botClient.SendMessage(update.Message.Chat, $"Задача '{id}' отмечена как выполненная '{update.Message.Text}", ct);
            }
        }

        static async Task ShowAllTasks(ITelegramBotClient botClient, Update update, List<ToDoItem> tasks, CancellationToken ct)
        {
            foreach (var task in tasks)
            {
                await botClient.SendMessage(update.Message.Chat, $" ({task.State}) {task.Name} - {task.CreatedAt} - {task.Id}", ct);
            }
        }

        async Task Report(ITelegramBotClient botClient, Update update, List<ToDoItem> tasks, CancellationToken ct)
        {
            try
            {
                long telegramUserID = update.Message.From.Id;
                User user = await _userService.GetUserByTelegramUserID(telegramUserID, ct);

                if (user != null)
                {

                    var report = await _toDoReportService.GetUserStats(user.UserId, ct);
                    await botClient.SendMessage(update.Message.Chat, $"Статистика по задачам на  {report.generatedAt:dd.MM.yyyy HH:mm:ss}. Всего {report.total}; " +
                        $"Завершённых: {report.completed}; Активных: {report.active}.", ct);
                }
            }
            catch (Exception ex) 
            {
                await HandleErrorAsync(botClient, ex, ct);
            }
        }

        async Task Find(ITelegramBotClient botClient, Update update, string input, CancellationToken ct)
        {
            try
            {
                long telegramUserID = update.Message.From.Id;
                User user = await _userService.GetUserByTelegramUserID(telegramUserID, ct);

                string namePrefix = input.Substring(6);
                ValidateString(namePrefix);
                var tasks = await _toDoService.Find(user, namePrefix, ct);
                if (tasks == null)
                {
                    await botClient.SendMessage(update.Message.Chat, $"Задачи не найдены!", ct);
                }
                else
                {
                    foreach (var task in tasks)
                    {
                        await botClient.SendMessage(update.Message.Chat, $"- '{task.Name}' - {task.CreatedAt} - {task.Id}", ct);
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, ct);
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

