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
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        
        public UpdateHandler(IUserService userService)
        {
            _userService = userService;
        }

        public void HandleUpdateAsync(ITelegramBotClient, Update)
        { try
            {
                string input = Console.ReadLine();
                switch (input?.ToLower().Split(' ')[0])
                {
                    case "/start":
                        Start();
                        break;
                    //case "/echo":
                      //  Echo(input);
                        break;
                    case "/help":
                        Help();
                        break;
                    case "/info":
                        Info();
                        break;
                    case "/addtask":
                        AddTask(Tasks);
                        break;
                    case "/showtasks":
                        Showtasks(Tasks);
                        break;
                    case "/removetask":
                        RemoveTasks(Tasks);
                        break;
                    case "/exit":
                        Exit(userName);
                        break;
                    default:
                        throw new ArgumentException("Введите одну из предложенных команд!");
                }
            }
            catch (Exception ex) { 
            }
        }

        private void Start()
        {
            while (true)
            {
                //ITelegramBotClient.SendMessage(chat,"Введите ваше имя: ");
                
                 var _userName = Update.Message.From.Username;
                var _userID = Update.Message.Form.UserId;
                ValidateString(_userName);
                IUserService.RegisterUser(_userName, _userID);
               ITelegramBotClient.SendMessage(chat,"Hello, " + _userName + "!");
               ITelegramBotClient.SendMessage(chat,"Введите максимально допустимое количество задач: ");
                while (true)
                {
                    var input = Console.ReadLine();
                    taskCountLimit = ParseAndValidateInt(input, min, max);
                    if (taskCountLimit > 0)
                    {
                       ITelegramBotClient.SendMessage(chat,"Максимальное число задач в списке - " + taskCountLimit);
                        break;
                    }

                }

                while (true)
                {
                   ITelegramBotClient.SendMessage(chat,"Введите максимальную длину описания задач: ");
                    var input = Console.ReadLine();
                    ValidateString(input);
                    taskLengthLimit = ParseAndValidateInt(input, min, max);
                    if (taskLengthLimit > 0)
                    {
                       ITelegramBotClient.SendMessage(chat,"Максимальная длина задачи - " + taskLengthLimit);
                       ITelegramBotClient.SendMessage(chat,"Для продолжения работы введите команду: ");
                        break;
                    }
                }
                EchoCommand = true;
                break;

            }
            return userName;
        }

        /*private void Echo(string input)
        {
            if (_EchoCommand)
            {
                string echoText = input.Substring(6);
                ValidateString(echoText);
               ITelegramBotClient.SendMessage(chat,echoText);
            }
        }*/

        static void Help()
        {
           ITelegramBotClient.SendMessage(chat,userName + ", чтобы начать работу, введите команду /start. Следуйте предложенным инструкциям, чтобы записать, изучить, сортировать или удалить словосочетание. " +
                "Чтобы добавить новую задачу введите /addtask, чтобы посмотреть список задач, введите /showtask, чтобы удалить задачу, введите /removetask");
        }

        static void Info()
        {
           ITelegramBotClient.SendMessage(chat,"Версия 1.0. Создано 24.02.2025");
        }

        static void Exit(string userName)
        {
           ITelegramBotClient.SendMessage(chat,"До новых встреч, " + userName + "!");
            Environment.Exit(0);
        }

        static void AddTask(List<string> Tasks)
        {
            if (Tasks.Count >= taskCountLimit)

            {
                throw new TaskCountLimitException(taskCountLimit);
            }

           ITelegramBotClient.SendMessage(chat,userName + ", введите описание задачи: ");
            string task = Console.ReadLine();
            ValidateString(task);
            if (task.Length > taskLengthLimit)
            {

                throw new TaskLengthLimitException(task.Length, taskLengthLimit);
            }
            if (Tasks.Contains(task))
            {
                throw new DuplicateTaskException(task);
            }
            Tasks.Add(task);
           ITelegramBotClient.SendMessage(chat,userName + ", задача " + "\"" + task + "\" " + "добавлена");

        }

        static void Showtasks(List<string> Tasks)
        {
            int taskNumber = 1;
           ITelegramBotClient.SendMessage(chat,userName + ", ваш список задач: ");
            if (Tasks.Count != 0)
            {
                for (int i = 0; i < Tasks.Count; i++)
                {

                   ITelegramBotClient.SendMessage(chat,taskNumber + ". " + Tasks[i]);
                    taskNumber++;
                }
            }
            elseITelegramBotClient.SendMessage(chat,userName + ", ваш список задач пуст!");

        }
        static void RemoveTasks(List<string> Tasks)
        {
            if (Tasks.Count != 0)
            {
               ITelegramBotClient.SendMessage(chat,userName + ", ваш список задач: ");
                int count = 1;
                foreach (var task in Tasks)
                {
                   ITelegramBotClient.SendMessage(chat,count + ". " + task);
                    count++;
                }
               ITelegramBotClient.SendMessage(chat,userName + ", введите номер задачи, которую нужно удалить: ");
                var taskNum = Console.ReadLine();
                ValidateString(taskNum);
                if (Int32.TryParse(taskNum, out int removeNum))
                {
                    if (removeNum > Tasks.Count || removeNum < 1)
                    {
                       ITelegramBotClient.SendMessage(chat,userName + ", задачи с номером " + removeNum + " не существует.");
                        return;
                    }
                    else
                    {
                       ITelegramBotClient.SendMessage(chat,userName + ", задача " + Tasks[removeNum - 1] + " удалена!");
                        Tasks.RemoveAt(removeNum - 1);
                        return;
                    }
                }

                elseITelegramBotClient.SendMessage(chat,userName + ", введите корректный номер задачи: ");

                for (int i = 0; i <= Tasks.Count; i++)
                {
                    if (i == removeNum - 1)
                    {
                        Tasks.RemoveAt(i);
                       ITelegramBotClient.SendMessage(chat,"Задача " + Tasks[i + 1] + " удалена!");
                        return;
                    }
                }
            }
            elseITelegramBotClient.SendMessage(chat,userName + ", ваш список задач пуст!:(");
            return;
        }

        static public int ParseAndValidateInt(string? str, int min, int max)
        {
            if (!Int32.TryParse(str, out int inputNum))
            {
                throw new ArgumentException(str + " не является целым числом!");
            }
            if (inputNum < min || inputNum > max)
            {
                throw new ArgumentException("Число должно быть в диапазоне от " + min + " до " + max);
            }
            return inputNum;
        }

        static public void ValidateString(string? str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Строка не может быть пустой!");
            }
        }

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
}
