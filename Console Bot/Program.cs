using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
namespace Console_Bot
{
    internal class Program
    {
        public static bool EchoCommand = false;
        public static string userName = string.Empty;
        List<string> Tasks = new List<string>();
        static void Main(string[] args)
        {
            Program program = new Program();
            Console.WriteLine("Добро пожаловать! Выберите команду: /start (для начала работы), /help (если требуется помощь), /info (о боте), /exit (для выхода)");
            while (true)
            {
               program.Input();
            }
        }

        bool Input()
        {
            string input = Console.ReadLine();

            if (input == "/start")
            {
                Start();
                return true;
            }
            else if (input.StartsWith("/echo") && EchoCommand)
            {
                Echo(input);
                return true;
            }
            else if (input == "/help")
            {
                Help();
                return true;
            }

            else if (input == "/info")
            {
                Info();
                return true;
            }
            else if (input == "/addtask")
            {
                AddTask(Tasks);
                return true;
            }

            else if (input =="/showtasks")
            {
                Showtasks(Tasks);
                return true;
            }

            else if (input == "/removetask")
            {
                RemoveTasks(Tasks);
                return true;
            }
            else if (input == "/exit")
            {
                Exit(userName);
                return false;
            }



            else
            {
                Console.WriteLine("Введите одну из предложенных команд");
                return true;
            }

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
        }

        static void AddTask(List<string> Tasks)
        {
            Console.WriteLine(userName + ", введите описание задачи: ");
            string task = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(task))
            {
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
                    Console.WriteLine(count + ". " + task );
                    count++;
                }
                Console.WriteLine(userName + ", введите номер задачи, которую нужно удалить: ");
                var taskNum = Console.ReadLine();
                if (Int32.TryParse(taskNum, out int removeNum))
                {
                    if (removeNum > Tasks.Count)
                    {
                        Console.WriteLine(userName +", задачи с номером " + removeNum + " не существует.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine(userName + ", задача " +Tasks[removeNum-1] +  " удалена!");
                        Tasks.RemoveAt(removeNum);
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
            else  Console.WriteLine(userName + ", ваш список задач пуст!:(");
            return;
        }
    }
}
