using System;
using System.Collections.Generic;
using System.Text;
using ReadLine.Abstractions;


namespace ReadLine
{
    public delegate bool CharMap(IKeyHandler keyHandler, ConsoleKeyInfo cki);


    internal sealed class KeyHandler : IKeyHandler
    {
        private readonly IConsole _console2;
        private readonly List<string> _history;
        private readonly Dictionary<string, Action> _keyActions;
        private readonly StringBuilder _text;
        private readonly bool _passwordMode;
        private string[] _completions;
        private int _completionsIndex;
        private int _completionStart;
        private int _cursorLimit;
        private int _cursorPos;
        private int _historyIndex;
        private ConsoleKeyInfo _keyInfo;


        private string BuildKeyInput() => _keyInfo.Modifiers != ConsoleModifiers.Control && _keyInfo.Modifiers != ConsoleModifiers.Shift ? _keyInfo.Key.ToString() : _keyInfo.Modifiers + _keyInfo.Key.ToString();
    

        internal KeyHandler(IConsole console, List<string> history, bool passwordMode, IAutoCompleteHandler autoCompleteHandler)
        {
            _console2 = console;
            _passwordMode = passwordMode;
            _history = history ?? new List<string>();
            _historyIndex = _history.Count;
            _text = new StringBuilder();
            _keyActions = new Dictionary<string, Action>
            {
                ["LeftArrow"] = MoveCursorLeft,
                ["Home"] = MoveCursorHome,
                ["End"] = MoveCursorEnd,
                ["ControlA"] = MoveCursorHome,
                ["ControlB"] = MoveCursorLeft,
                ["RightArrow"] = MoveCursorRight,
                ["ControlF"] = MoveCursorRight,
                ["ControlE"] = MoveCursorEnd,
                ["Backspace"] = Backspace,
                ["Delete"] = Delete,
                ["ControlD"] = Delete,
                ["ControlH"] = Backspace,
                ["ControlL"] = ClearLine,
                ["Escape"] = ClearLine,
                ["UpArrow"] = PrevHistory,
                ["ControlP"] = PrevHistory,
                ["DownArrow"] = NextHistory,
                ["ControlN"] = NextHistory,
                ["ControlU"] = () => Backspace(_cursorPos),
                ["ControlK"] = () =>
                {
                    var pos = _cursorPos;
                    MoveCursorEnd();
                    Backspace(_cursorPos - pos);
                },
                ["ControlW"] = () =>
                {
                    while (!IsStartOfLine() && _text[_cursorPos - 1] != ' ')
                        Backspace();
                },
                ["ControlT"] = TransposeChars,
                ["Tab"] = () =>
                {
                    if (IsInAutoCompleteMode())
                    {
                        NextAutoComplete();
                    }
                    else
                    {
                        if (autoCompleteHandler == null || !IsEndOfLine())
                            return;

                        var text = _text.ToString();

                        _completionStart = text.LastIndexOfAny(autoCompleteHandler.Separators);
                        _completionStart = _completionStart == -1 ? 0 : _completionStart + 1;

                        _completions = autoCompleteHandler.GetSuggestions(text, _completionStart);
                        _completions = _completions?.Length == 0 ? null : _completions;

                        if (_completions == null)
                            return;

                        StartAutoComplete();
                    }
                },
                ["ShiftTab"] = () =>
                {
                    if (IsInAutoCompleteMode())
                        PreviousAutoComplete();
                }
            };
        }


        public override string ToString() => _text.ToString();


        public bool IsStartOfLine() => _cursorPos == 0;


        public bool IsEndOfLine() => _cursorPos == _cursorLimit;


        public bool IsStartOfBuffer() => _console2.CursorLeft == 0;


        public bool IsEndOfBuffer() => _console2.CursorLeft == _console2.BufferWidth - 1;


        public bool IsInAutoCompleteMode() => _completions != null;


        public void MoveCursorLeft() => MoveCursorLeft(1);


        public void MoveCursorLeft(int count)
        {
            if (count > _cursorPos)
                count = _cursorPos;

            if (count > _console2.CursorLeft)
                _console2.SetCursorPosition(_console2.BufferWidth - 1, _console2.CursorTop);
            else
                _console2.SetCursorPosition(_console2.CursorLeft - count, _console2.CursorTop);

            _cursorPos -= count;
        }


        public void MoveCursorHome()
        {
            while (!IsStartOfLine())
                MoveCursorLeft();
        }


        public void MoveCursorRight()
        {
            if (IsEndOfLine())
                return;

            if (IsEndOfBuffer())
                _console2.SetCursorPosition(0, _console2.CursorTop + 1);
            else
                _console2.SetCursorPosition(_console2.CursorLeft + 1, _console2.CursorTop);

            _cursorPos++;
        }


        public void MoveCursorEnd()
        {
            while (!IsEndOfLine())
                MoveCursorRight();
        }


        public void ClearLine()
        {
            MoveCursorEnd();
            Backspace(_cursorPos);
        }


        public void WriteNewString(string str)
        {
            ClearLine();
            WriteString(str);
        }


        public void WriteString(string str)
        {
            foreach (var character in str)
                WriteChar(character);
        }


        public void WriteChar() => WriteChar(_keyInfo.KeyChar);


        public void WriteChar(char c)
        {
            if (IsEndOfLine()) {
                _text.Append(c);
                _cursorPos++;

                if (!_passwordMode)
                    _console2.Write(c.ToString());
            }
            else
            {
                var left = _console2.CursorLeft;
                var top = _console2.CursorTop;
                var str = _text.ToString().Substring(_cursorPos);
                _text.Insert(_cursorPos, c);
                
                if (!_passwordMode)
                {
                  _console2.Write(c + str);
                  _console2.SetCursorPosition(left, top);
                }

                MoveCursorRight();
            }

            _cursorLimit++;
        }


        public void Backspace() => Backspace(1);


        public void Backspace(int count)
        {
            if (count > _cursorPos)
                count = _cursorPos;

            MoveCursorLeft(count);
            var index = _cursorPos;
            _text.Remove(index, count);
            var replacement = _text.ToString().Substring(index);
            var left = _console2.CursorLeft;
            var top = _console2.CursorTop;
            var spaces = new string(' ', count);
            _console2.Write($"{replacement}{spaces}");
            _console2.SetCursorPosition(left, top);
            _cursorLimit -= count;
        }


        public void Delete()
        {
            if (IsEndOfLine())
                return;

            var index = _cursorPos;
            _text.Remove(index, 1);
            var replacement = _text.ToString().Substring(index);
            var left = _console2.CursorLeft;
            var top = _console2.CursorTop;
            _console2.Write($"{replacement} ");
            _console2.SetCursorPosition(left, top);
            _cursorLimit--;
        }


        public void TransposeChars()
        {
            // local helper functions
            bool AlmostEndOfLine() => _cursorLimit - _cursorPos == 1;

            int IncrementIf(Func<bool> expression, int index) => expression() ? index + 1 : index;

            int DecrementIf(Func<bool> expression, int index) => expression() ? index - 1 : index;

            if (IsStartOfLine())
                return;

            var firstIdx = DecrementIf(IsEndOfLine, _cursorPos - 1);
            var secondIdx = DecrementIf(IsEndOfLine, _cursorPos);

            var secondChar = _text[secondIdx];
            _text[secondIdx] = _text[firstIdx];
            _text[firstIdx] = secondChar;

            var left = IncrementIf(AlmostEndOfLine, _console2.CursorLeft);
            var cursorPosition = IncrementIf(AlmostEndOfLine, _cursorPos);

            WriteNewString(_text.ToString());

            _console2.SetCursorPosition(left, _console2.CursorTop);
            _cursorPos = cursorPosition;

            MoveCursorRight();
        }


        public void StartAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

            _completionsIndex = 0;

            WriteString(_completions[_completionsIndex]);
        }


        public void NextAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

            _completionsIndex++;

            if (_completionsIndex == _completions.Length)
                _completionsIndex = 0;

            WriteString(_completions[_completionsIndex]);
        }


        public void PreviousAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

            _completionsIndex--;

            if (_completionsIndex == -1)
                _completionsIndex = _completions.Length - 1;

            WriteString(_completions[_completionsIndex]);
        }


        public void PrevHistory()
        {
            if (_historyIndex > 0)
            {
                _historyIndex--;
                WriteNewString(_history[_historyIndex]);
            }
        }


        public void NextHistory()
        {
            if (_historyIndex < _history.Count)
            {
                _historyIndex++;
                if (_historyIndex == _history.Count)
                    ClearLine();
                else
                    WriteNewString(_history[_historyIndex]);
            }
        }


        internal void Handle(ConsoleKeyInfo keyInfo, CharMap callback = null)
        {
            _keyInfo = keyInfo;

            // If in auto complete mode and Tab wasn't pressed
            if (IsInAutoCompleteMode() && _keyInfo.Key != ConsoleKey.Tab) {
                // ResetAutoComplete
                _completions = null;
                _completionsIndex = 0;
            }

            _keyActions.TryGetValue(BuildKeyInput(), out var action);
            if (action != null)
              action.Invoke();
            else {
              if (callback == null || !callback.Invoke(this, keyInfo))
                WriteChar();
            }
        }
    }
}