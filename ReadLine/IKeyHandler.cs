namespace ReadLine
{
    public interface IKeyHandler
    {
        string ToString();

        bool IsStartOfLine();

        bool IsEndOfLine();

        bool IsStartOfBuffer();

        bool IsEndOfBuffer();

        bool IsInAutoCompleteMode();

        void MoveCursorLeft();

        void MoveCursorLeft(int count);

        void MoveCursorHome();
      
        void MoveCursorRight();

        void MoveCursorEnd();
      
        void ClearLine();

        void WriteNewString(string str);

        void WriteString(string str);

        void WriteChar();

        void WriteChar(char c);

        void Backspace();

        void Backspace(int count);

        void Delete();
        
        void TransposeChars();

        void StartAutoComplete();

        void NextAutoComplete();

        void PreviousAutoComplete();
        void PrevHistory();

        void NextHistory();
    }
}