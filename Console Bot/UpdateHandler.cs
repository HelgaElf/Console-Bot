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
    class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
        }
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update) 
        {

            try
            {
                string input = Console.ReadLine();
                string command = input?.ToLower().Split(' ')[0];

                if (!Program.active && command != "/help" && command != "/info" && command != "/start")
                {
                    botClient.SendMessage(update.Message.Chat, $"Для начала работы введите команду /start'{update.Message.Text}'");
                    return; 
                }

                switch (command)
                {
                    case "/start":
                        Start(botClient, update);
                        break;
                    //case "/echo":
                      //  Echo(input);
                        break;
                    case "/help":
                        Help(botClient, update);
                        break;
                    case "/info":
                        Info(botClient, update);
                        break;
                    case "/addtask":
                        AddTask(botClient, update, input);
                        break;
                    case "/showtasks":
                        Showtasks(botClient, update, Program.Tasks);
                        break;
                    case "/removetask":
                        RemoveTasks(botClient, update, input);
                        break;
                   // case "/exit":
                    //    Exit(userName);
                    //    break;
                    case "/completetask":
                    CompleteTask(botClient, update, input);
                        break;
                   case "/showalltasks":
                       ShowAllTasks(botClient, update, Program.Tasks);
                       break;
                    case "/report":
                        Report(botClient, update, Program.Tasks);
                        break;
                    case "/find":
                        Find(botClient, update, input); //какой тут нужен метод?
                        break;
                    default:
                        throw new ArgumentException("Введите одну из предложенных команд!");
                }
            }
            catch (Exception ex)
            { 

            }
        }

        private void Start(ITelegramBotClient botClient, Update update)
        {
            string userName = update.Message.From.Username ?? "User";
            long userID = update.Message.From.Id;

            User newUser =_userService.RegisterUser(userID, userName);

            Program.active = true;
            Program.isRegisteredUser = newUser.TelegramUserId;

            botClient.SendMessage(update.Message.Chat, $"Пользователь зарегистрирован '{update.Message.Text}'");

        }

        static void Help(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $" чтобы начать работу, введите команду /start "
                +"Чтобы добавить новую задачу введите /addtask \"Имя задачи\", чтобы посмотреть список задач, введите /showtask, чтобы удалить задачу, введите /removetask");
        }

        static void Info(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Версия 0.1.5 Создано 24.02.2025 {update.Message.Text}");
        }

        //static void Exit(string userName)
        //{
        //   ITelegramBotClient.SendMessage(chat,"До новых встреч, " + userName + "!");
        //    Environment.Exit(0);
        //}

        void AddTask(ITelegramBotClient botClient, Update update, string newTask)
        {
            string userName = update.Message.From.Username ?? "User";
            long userID = update.Message.From.Id;
            var user = new User
            {
                TelegramUserId = userID,
                TelegramUserName = userName,
            };

            string taskText = newTask.Substring(9);
            ValidateString(taskText);

            ToDoItem task = _toDoService.Add(user, taskText);

            botClient.SendMessage(update.Message.Chat, $"Пользователь '{userName}' задача '{task.Id}' добавлена '{update.Message.Text}'");

        }

        static void Showtasks(ITelegramBotClient botClient, Update update, List<ToDoItem> tasks)
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
                botClient.SendMessage(update.Message.Chat, $"Нет активных задач!");
            }
            else {
                foreach (var task in tasks)
                {
                    botClient.SendMessage(update.Message.Chat, $"- '{task.Name}' - {task.CreatedAt} - {task.Id}"  );
                }
            }
        }
         void RemoveTasks(ITelegramBotClient botClient, Update update, string input)
         {
            if (Program.Tasks.Count != 0)
            {
                string taskGuid = input.Substring(12);
                if (Guid.TryParse(taskGuid, out var id))
                {
                    _toDoService.Delete(id);
                    botClient.SendMessage(update.Message.Chat, $"Задача '{id}' удалена '{update.Message.Text}");
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

         void CompleteTask(ITelegramBotClient botClient, Update update, string input)
        {
            string taskGuid = input.Substring(14);
            if (Guid.TryParse(taskGuid, out var id))
            {
                _toDoService.MarkCompleted(id);
                botClient.SendMessage(update.Message.Chat, $"Задача '{id}' отмечена как выполненная '{update.Message.Text}");
            }
        }

        static void ShowAllTasks (ITelegramBotClient botClient, Update update, List<ToDoItem> tasks)
        {
            foreach (var task in tasks)
            {
                botClient.SendMessage(update.Message.Chat, $" ({task.State}) {task.Name} - {task.CreatedAt} - {task.Id}");
            }
        }

         void Report(ITelegramBotClient botClient, Update update, List<ToDoItem> tasks)
        {
            long telegramUserID = update.Message.From.Id;
            User user = _userService.GetUserByTelegramUserID(telegramUserID);

            if (user != null)
            {
               
                var report = _toDoReportService.GetUserStats(user.UserId);
                botClient.SendMessage(update.Message.Chat, $"Статистика по задачам на  {report.generatedAt :dd.MM.yyyy HH:mm:ss}. Всего {report.total}; " +
                    $"Завершённых: {report.completed}; Активных: {report.active}.");
            }
        }

         void Find (ITelegramBotClient botClient, Update update, string input)
         {
            long telegramUserID = update.Message.From.Id;
            User user = _userService.GetUserByTelegramUserID(telegramUserID);

            string namePrefix = input.Substring(6);
            ValidateString(namePrefix);
            var tasks = _toDoService.Find(user, namePrefix);
            if (tasks == null) 
            {
                botClient.SendMessage(update.Message.Chat, $"Задачи не найдены!");
            }
            else
            {
                foreach (var task in tasks)
                {
                    botClient.SendMessage(update.Message.Chat, $"- '{task.Name}' - {task.CreatedAt} - {task.Id}");
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

