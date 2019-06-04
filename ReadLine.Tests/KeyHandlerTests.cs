using System;
using System.Collections.Generic;
using System.Linq;
using ReadLine.Abstractions;
using NUnit.Framework;
using static ReadLine.Tests.ConsoleKeyInfoExtensions;


namespace ReadLine.Tests
{
    internal sealed class KeyHandlerTests
    {
        private KeyHandler _keyHandler;
        private readonly List<string> _history;
        private readonly AutoCompleteHandler _autoCompleteHandler;
        private readonly string[] _completions;
        private readonly IConsole _console;

        internal KeyHandlerTests()
        {
            _autoCompleteHandler = new AutoCompleteHandler();
            _completions = _autoCompleteHandler.GetSuggestions("", 0);
            _history = new List<string>(new[] { "dotnet run", "git init", "clear" });

            _console = new Console2();
            _keyHandler = new KeyHandler(_console, _history, null);

            "Hello".Select(c => c.ToConsoleKeyInfo())
                    .ToList()
                    .ForEach(_keyHandler.Handle);
        }

        [Test]
        public void TestWriteChar()
        {
            Assert.AreEqual("Hello", _keyHandler.ToString());
            
            " World".Select(c => c.ToConsoleKeyInfo())
                    .ToList()
                    .ForEach(_keyHandler.Handle);
                    
            Assert.AreEqual("Hello World", _keyHandler.ToString());
        }

        [Test]
        public void TestBackspace()
        {
            _keyHandler.Handle(Backspace);
            Assert.AreEqual("Hell", _keyHandler.ToString());
        }

        [Test]
        public void TestDelete()
        {
            new List<ConsoleKeyInfo>() { LeftArrow, Delete }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hell", _keyHandler.ToString());
        }

        [Test]
        public void TestDelete_EndOfLine()
        {
            _keyHandler.Handle(Delete);
            Assert.AreEqual("Hello", _keyHandler.ToString());
        }

        [Test]
        public void TestControlH()
        {
            _keyHandler.Handle(CtrlH);
            Assert.AreEqual("Hell", _keyHandler.ToString());
        }

        [Test]
        public void TestControlT()
        {
            var initialCursorCol = _console.CursorLeft;
            _keyHandler.Handle(CtrlT);

            Assert.AreEqual("Helol", _keyHandler.ToString());
            Assert.AreEqual(initialCursorCol, _console.CursorLeft);
        }

        [Test]
        public void TestControlT_LeftOnce_CursorMovesToEnd()
        {
            var initialCursorCol = _console.CursorLeft;

            new List<ConsoleKeyInfo>() { LeftArrow, CtrlT }
                .ForEach(_keyHandler.Handle);
            
            Assert.AreEqual("Helol", _keyHandler.ToString());
            Assert.AreEqual(initialCursorCol, _console.CursorLeft);
        }

        [Test]
        public void TestControlT_CursorInMiddleOfLine()
        {
            Enumerable
                .Repeat(LeftArrow, 3)
                .ToList()
                .ForEach(_keyHandler.Handle);

            var initialCursorCol = _console.CursorLeft;

            _keyHandler.Handle(CtrlT);

            Assert.AreEqual("Hlelo", _keyHandler.ToString());
            Assert.AreEqual(initialCursorCol + 1, _console.CursorLeft);
        }

        [Test]
        public void TestControlT_CursorAtBeginningOfLine_HasNoEffect()
        {
            _keyHandler.Handle(CtrlA);

            var initialCursorCol = _console.CursorLeft;

            _keyHandler.Handle(CtrlT);

            Assert.AreEqual("Hello", _keyHandler.ToString());
            Assert.AreEqual(initialCursorCol, _console.CursorLeft);
        }

        [Test]
        public void TestHome()
        {
            new List<ConsoleKeyInfo>() { Home, 'S'.ToConsoleKeyInfo() }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("SHello", _keyHandler.ToString());
        }

        [Test]
        public void TestControlA()
        {
            new List<ConsoleKeyInfo>() { CtrlA, 'S'.ToConsoleKeyInfo() }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("SHello", _keyHandler.ToString());
        }

        [Test]
        public void TestEnd()
        {
            new List<ConsoleKeyInfo>() { Home, End, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hello!", _keyHandler.ToString());
        }

        [Test]
        public void TestControlE()
        {
            new List<ConsoleKeyInfo>() { CtrlA, CtrlE, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hello!", _keyHandler.ToString());
        }

        [Test]
        public void TestLeftArrow()
        {
            " N".Select(c => c.ToConsoleKeyInfo())
                .Prepend(LeftArrow)
                .ToList()
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hell No", _keyHandler.ToString());
        }

        [Test]
        public void TestControlB()
        {
            " N".Select(c => c.ToConsoleKeyInfo())
                .Prepend(CtrlB)
                .ToList()
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hell No", _keyHandler.ToString());
        }

        [Test]
        public void TestRightArrow()
        {
            new List<ConsoleKeyInfo>() { LeftArrow, RightArrow, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hello!", _keyHandler.ToString());
        }

        [Test]
        public void TestControlD()
        {
            Enumerable.Repeat(LeftArrow, 4)
                    .Append(CtrlD)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hllo", _keyHandler.ToString());
        }

        [Test]
        public void TestControlF()
        {
            new List<ConsoleKeyInfo>() { LeftArrow, CtrlF, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hello!", _keyHandler.ToString());
        }

        [Test]
        public void TestControlL()
        {
            _keyHandler.Handle(CtrlL);
            Assert.AreEqual(string.Empty, _keyHandler.ToString());
        }

        [Test]
        public void TestUpArrow()
        {
            _history.AsEnumerable().Reverse().ToList().ForEach((history) => {
                _keyHandler.Handle(UpArrow);
                Assert.AreEqual(history, _keyHandler.ToString());
            });
        }

        [Test]
        public void TestControlP()
        {
            _history.AsEnumerable().Reverse().ToList().ForEach((history) => {
                _keyHandler.Handle(CtrlP);
                Assert.AreEqual(history, _keyHandler.ToString());
            });
        }

        [Test]
        public void TestDownArrow()
        {
            Enumerable.Repeat(UpArrow, _history.Count)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            _history.ForEach( history => {
                Assert.AreEqual(history, _keyHandler.ToString());
                _keyHandler.Handle(DownArrow);
            });
        }

        [Test]
        public void TestControlN()
        {
            Enumerable.Repeat(UpArrow, _history.Count)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            _history.ForEach( history => {
                Assert.AreEqual(history, _keyHandler.ToString());
                _keyHandler.Handle(CtrlN);
            });
        }

        [Test]
        public void TestControlU()
        {
            _keyHandler.Handle(LeftArrow);
            _keyHandler.Handle(CtrlU);

            Assert.AreEqual("o", _keyHandler.ToString());

            _keyHandler.Handle(End);
            _keyHandler.Handle(CtrlU);

            Assert.AreEqual(string.Empty, _keyHandler.ToString());
        }

        [Test]
        public void TestControlK()
        {
            _keyHandler.Handle(LeftArrow);
            _keyHandler.Handle(CtrlK);

            Assert.AreEqual("Hell", _keyHandler.ToString());

            _keyHandler.Handle(Home);
            _keyHandler.Handle(CtrlK);

            Assert.AreEqual(string.Empty, _keyHandler.ToString());
        }

        [Test]
        public void TestControlW()
        {
            " World".Select(c => c.ToConsoleKeyInfo())
                    .Append(CtrlW)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            Assert.AreEqual("Hello ", _keyHandler.ToString());

            _keyHandler.Handle(Backspace);
            _keyHandler.Handle(CtrlW);

            Assert.AreEqual(string.Empty, _keyHandler.ToString());
        }

        [Test]
        public void TestTab()
        {
            _keyHandler.Handle(Tab);
            // Nothing happens when no auto complete handler is set
            Assert.AreEqual("Hello", _keyHandler.ToString());

            _keyHandler = new KeyHandler(new Console2(), _history, _autoCompleteHandler);

            "Hi ".Select(c => c.ToConsoleKeyInfo()).ToList().ForEach(_keyHandler.Handle);

            _completions.ToList().ForEach(completion => {
                _keyHandler.Handle(Tab);
                Assert.AreEqual($"Hi {completion}", _keyHandler.ToString());
            });
        }

        [Test]
        public void TestBackwardsTab()
        {
            _keyHandler.Handle(Tab);

            // Nothing happens when no auto complete handler is set
            Assert.AreEqual("Hello", _keyHandler.ToString());

            _keyHandler = new KeyHandler(new Console2(), _history, _autoCompleteHandler);

            "Hi ".Select(c => c.ToConsoleKeyInfo()).ToList().ForEach(_keyHandler.Handle);

            // Bring up the first Autocomplete
            _keyHandler.Handle(Tab);

            _completions.Reverse().ToList().ForEach(completion => {
                _keyHandler.Handle(ShiftTab);
                Assert.AreEqual($"Hi {completion}", _keyHandler.ToString());
            });
        }

        [Test]
        public void MoveCursorThenPreviousHistory()
        {
            _keyHandler.Handle(LeftArrow);
            _keyHandler.Handle(UpArrow);

            Assert.AreEqual("clear", _keyHandler.ToString());
        }
    }
}