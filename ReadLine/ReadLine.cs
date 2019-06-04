using System;
using System.Collections.Generic;
using ReadLine.Abstractions;


namespace ReadLine
{
    public static class ReadLine
    {
        private static readonly List<string> History = new List<string>();


        public static IAutoCompleteHandler AutoCompletionHandler { private get; set; }


        public static void AddHistory(params string[] text) => History.AddRange(text);


        public static List<string> GetHistory() => History;


        public static void ClearHistory() => History.Clear();


        public static string Read(string prompt, bool passwordMode) => Read(prompt, string.Empty, passwordMode);
        public static string Read(string prompt = "", string @default = "", bool passwordMode = false)
        {
            Console.Write(prompt);

            var console2 = new Console2 {
                PasswordMode = passwordMode
            };
            var keyHandler = new KeyHandler(console2, History, AutoCompletionHandler);

            #region GetText
            // ----------------------------------------------------------------
            var keyInfo = Console.ReadKey(intercept: !passwordMode);
            
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyHandler.Handle(keyInfo);
                keyInfo = Console.ReadKey(intercept: !passwordMode);
            }
            
            var text = keyHandler.ToString();
            // ----------------------------------------------------------------
            #endregion GetText

            if (!passwordMode) {
                if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(@default))
                    text = @default;
                else
                    History.Add(text);
            }

            return text;
        }
    }
}