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


        public static string Read(string prompt, CharMap callback) => Read(prompt, string.Empty, false, callback);
        public static string Read(string prompt, bool passwordMode, CharMap callback = null) => Read(prompt, string.Empty, passwordMode, callback);
        public static string Read(string prompt = "", string @default = "", bool passwordMode = false, CharMap callback = null)
        {
            Console.Write(prompt);

            var console2 = new Console2 {
                PasswordMode = passwordMode
            };
            var keyHandler = new KeyHandler(console2, History, passwordMode, AutoCompletionHandler);

            #region GetText
            // ----------------------------------------------------------------
            var keyInfo = Console.ReadKey(intercept: true);
            
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyHandler.Handle(keyInfo, callback);
                keyInfo = Console.ReadKey(intercept: true);
            }
            
            var text = string.Empty;
            // ----------------------------------------------------------------
            #endregion GetText

            if (!passwordMode) {
                text = keyHandler.ToString();
                if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(@default))
                    text = @default;
                else
                    History.Add(text);
            }

            return text;
        }
    }
}