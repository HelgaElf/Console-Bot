using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
            //try
            //{
                Program program = new Program();
                Console.WriteLine("Добро пожаловать! Выберите команду: /start (для начала работы), /help (если требуется помощь), /info (о боте), /exit (для выхода)");
                while (true)
                {
                try
                {
                    program.Input();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType);
                    Console.WriteLine("Ошибка: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine("Выберите команду: /start (для начала работы), /help (если требуется помощь), /info (о боте), /exit (для выхода)");
                }
            }           
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.GetType);
            //    Console.WriteLine("Ошибка: " + ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //    Console.WriteLine(ex.InnerException);
            //}
        }

     

        bool Input()
        {
            //try
            //{
                string input = Console.ReadLine();

                switch (input)
                {
                    case "/start":
                        Start();
                        break;

                    case "/echo":
                        Echo(input);
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
            //}

            //catch (ArgumentException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return true;
            //}

            return true;
        }
        static string Start()
        {
            while (true)
            {
                Console.WriteLine("Введите ваше имя: ");
                userName = Console.ReadLine();
                ValidateString(userName);
                
                    Console.WriteLine("Hello, " + userName + "!");
                    Console.WriteLine("Введите максимально допустимое количество задач: ");
                    while (true)
                    {
                        var input = Console.ReadLine();
                        taskCountLimit = ParseAndValidateInt(input, min, max);
                        if(taskCountLimit > 0)
                        {
                            Console.WriteLine("Максимальное число задач в списке - " + taskCountLimit);
                        break;
                        }
   
                    }
                
                    while (true) 
                    {
                     Console.WriteLine("Введите максимальную длину описания задач: ");
                     var input = Console.ReadLine();
                     ValidateString(input);
                    taskLengthLimit = ParseAndValidateInt(input, min, max);
                        if (taskLengthLimit > 0)
                        {
                        Console.WriteLine("Максимальная длина задачи - " + taskLengthLimit);
                        Console.WriteLine("Для продолжения работы введите команду: ");
                            break;
                        }                         
                    }
                    EchoCommand = true;
                    break;
                
            }
            return userName;
        }

        static void Echo(string input)
        {
            if (EchoCommand)
            {
                string echoText = input.Substring(6);
                ValidateString(echoText);

                Console.WriteLine(echoText);
            }
        }

        static void Help()
        {
            Console.WriteLine(userName + ", чтобы начать работу, введите команду /start. Следуйте предложенным инструкциям, чтобы записать, изучить, сортировать или удалить словосочетание. " +
                "Чтобы добавить новую задачу введите /addtask, чтобы посмотреть список задач, введите /showtask, чтобы удалить задачу, введите /removetask");
        }

        static void Info()
        {
            Console.WriteLine("Версия 1.0. Создано 24.02.2025");
        }

        static void Exit(string userName)
        {
            Console.WriteLine("До новых встреч, " + userName + "!");
           Environment.Exit(0);
        }

        static void AddTask(List<string> Tasks)
        {
                if (Tasks.Count >= taskCountLimit)

                {
                    throw new TaskCountLimitException(taskCountLimit);
                }
            
            Console.WriteLine(userName + ", введите описание задачи: ");
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
            Console.WriteLine(userName + ", задача " + "\"" + task + "\" " + "добавлена");
  
        }

        static void Showtasks(List<string> Tasks)
        {
            int taskNumber = 1;
            Console.WriteLine(userName + ", ваш список задач: ");
            if (Tasks.Count != 0)
            {
                for (int i = 0; i < Tasks.Count; i++)
                {

                    Console.WriteLine(taskNumber + ". " + Tasks[i]);
                    taskNumber++;
                }
            }
            else Console.WriteLine(userName + ", ваш список задач пуст!");

        }
        static void RemoveTasks(List<string> Tasks)
        {
            if (Tasks.Count != 0)
            {
                Console.WriteLine(userName + ", ваш список задач: ");
                int count = 1;
                foreach (var task in Tasks)
                {
                    Console.WriteLine(count + ". " + task);
                    count++;
                }
                Console.WriteLine(userName + ", введите номер задачи, которую нужно удалить: ");
                var taskNum = Console.ReadLine();
                ValidateString(taskNum);
                if (Int32.TryParse(taskNum, out int removeNum))
                {
                    if (removeNum > Tasks.Count || removeNum < 1)
                    {
                        Console.WriteLine(userName + ", задачи с номером " + removeNum + " не существует.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine(userName + ", задача " + Tasks[removeNum - 1] + " удалена!");
                        Tasks.RemoveAt(removeNum - 1);
                        return;
                    }
                }

                else Console.WriteLine(userName + ", введите корректный номер задачи: ");

                for (int i = 0; i <= Tasks.Count; i++)
                {
                    if (i == removeNum - 1)
                    {
                        Tasks.RemoveAt(i);
                        Console.WriteLine("Задача " + Tasks[i + 1] + " удалена!");
                        return;
                    }
                }
            }
            else Console.WriteLine(userName + ", ваш список задач пуст!:(");
            return;
        }

       static public int ParseAndValidateInt(string? str, int  min, int max)
        {
            if (!Int32.TryParse(str, out int inputNum))
            {
                throw new ArgumentException(str + " не является целым числом!");
            }
            if (inputNum < min || inputNum > max)
            {
                throw new ArgumentException("Число должно быть в диапазоне от " +min + " до " + max);
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
        public TaskLengthLimitException(int taskLength,  int taskLengthLimit)
            : base($"Длина задачи '{ taskLength}' превышает максимально допустимое значение {taskLengthLimit}")
        {
        }
    }
}
