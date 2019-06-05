using System;
using System.Linq;
using System.Text;


namespace ReadLine.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ReadLine Library Demo");
            Console.WriteLine("---------------------");
            Console.WriteLine();

            string[] history =
            {
                "ls -a",
                "dotnet run",
                "git init"
            };
            ReadLine.AddHistory(history);

            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
            
            var input = ReadLine.Read("(prompt)> ", Callback);
            Console.WriteLine(Environment.NewLine + input);

            var bytes = Encoding.ASCII.GetBytes(input);
            Console.WriteLine(string.Join(" ", bytes.Select(x => x.ToString("x2"))));

            input = ReadLine.Read("Enter Password> ", true);
            Console.WriteLine(input);
        }


        private static bool Callback(IKeyHandler keyHandler, ConsoleKeyInfo cki)
        {
            bool isHandled;

            switch (cki.KeyChar) {
              case '\b':
                keyHandler.Backspace();
                isHandled = true;
                break;
              default:
                isHandled = false;
                break;
            }

            return isHandled;
        }
    }
}