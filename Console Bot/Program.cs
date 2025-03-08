namespace Console_Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string userName = string.Empty;
            Console.WriteLine("Добро пожаловать! Выберите команду: /start (для начала работы), /help (если требуется помощь), /info (о боте), /exit (для выхода)");
            //Console.ReadLine();
            bool EchoCommand = false;
            while (true)
            {
                
                string input = Console.ReadLine();
                if (input == "/start")                    
                {
                    Console.WriteLine("Введите ваше имя: ");
                    userName = Console.ReadLine();
                    Console.WriteLine("Hello, " + userName + "!");
                    EchoCommand = true;
                }

                if (input.StartsWith("/echo") && EchoCommand)
                {
                    //var len = input.Length;
                    //string echoText = input.Substring(2);
                    //Console.WriteLine(echoText);
                    if (echoText != string.IsEmpty(input))
                        Console.WriteLine(echoText);
                    else Console.WriteLine("Введите текст для эха!");
                }
                if  (input == "/help")
                {
                    Console.WriteLine("Чтобы начать работу, введите команду /start. Следуйте предложенным интструкциям, чтобы записать, изучить, сортировать или удалить словосочетание");
                }

                if(input == "/info")
                {
                    Console.WriteLine("Версия 1.0. Создано 24.02.2025");
                }

                if(input == "/exit")
                {
                    Console.WriteLine("До новых встреч, " + userName + "!");
                    break;
                }

                else
                {
                    Console.WriteLine("Введите одну из предложенных команд");
                }

             
            }
        }
    }
}
