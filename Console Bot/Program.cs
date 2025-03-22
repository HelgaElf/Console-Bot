using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
namespace Console_Bot
{
    internal class Program
    {
        public static bool EchoCommand = false;
        public static string userName = string.Empty;
        public static List<string> Tasks = new List<string>();
        public static int taskCountLimit;
        public static int taskLengthLimit;
        static void Main(string[] args)
        {
            try
            {
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
                    }
                }           
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

     

        bool Input()
        {
            try
            {
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
            }

            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }

            return true;
        }
        static string Start()
        {
            while (true)
            {
                Console.WriteLine("Введите ваше имя: ");
                userName = Console.ReadLine();
                if (userName != string.Empty)
                {
                    Console.WriteLine("Hello, " + userName + "!");
                    Console.WriteLine("Введите максимально допустимое количество задач: ");
                    while (true)
                    {
                        try
                        {
                            var input = Console.ReadLine();

                            if (Int32.TryParse(input, out taskCountLimit) && taskCountLimit < 100 && taskCountLimit >= 1)
                            {

                                Console.WriteLine("Максимальное число задач в списке - " + taskCountLimit);
                                break;
                            }
                            else
                            {
                                throw new ArgumentException("Введите число от 1 до 100!");
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }

                                          
                        while (true) 
                    {
                        try
                        {

                            Console.WriteLine("Введите максимальную длину описания задач: ");
                            var input = Console.ReadLine();
                            if (!Int32.TryParse(input, out taskLengthLimit) || taskLengthLimit == 0 || taskLengthLimit > 100)
                            {
                                throw new ArgumentException("Введите число от 1 до 100!");
                                
                            }
                            
                                Console.WriteLine("Максимальная длина задачи - " + taskLengthLimit);
                            break;
                          
                        }
                        catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
                    }

                   

                    EchoCommand = true;
                    break;
                }

                else
                {
                    Console.WriteLine("Имя пользователя не может быть пустым!");
                }
            
            }
            return userName;
        }

        static void Echo(string input)
        {
            if (EchoCommand)
            {
                string echoText = input.Substring(6);
                if (!string.IsNullOrEmpty(echoText))
                {

                    Console.WriteLine(echoText);
                }
                else
                {
                    Console.WriteLine(userName + ", введите текст для эха!");
                }
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
            try
            {
                if (Tasks.Count >= taskCountLimit)

                {
                    throw new TaskCountLimitException(taskCountLimit);

                }
            }

            catch (TaskCountLimitException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine(userName + ", введите описание задачи: ");
            string task = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(task))
            {
                try
                {

                    if (task.Length > taskLengthLimit)
                    {

                        throw new TaskLengthLimitException(task.Length, taskLengthLimit);
                    }
                }
                catch (TaskLengthLimitException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            
                try
                {
                    if (Tasks.Contains(task))
                        throw new DuplicateTaskException(task);
                }

                catch (DuplicateTaskException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                Tasks.Add(task);
                Console.WriteLine(userName + ", задача " + "\"" + task + "\" " + "добавлена");
  
            }
            else Console.WriteLine(userName + ", задача не может быть пустой.");
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
