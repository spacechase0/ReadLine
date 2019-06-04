using System;


namespace ReadLine.Tests
{
    internal static class ConsoleKeyInfoExtensions
    {
        internal static readonly ConsoleKeyInfo Backspace = new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false);
        internal static readonly ConsoleKeyInfo Delete = new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);

        internal static readonly ConsoleKeyInfo Home = new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false);
        internal static readonly ConsoleKeyInfo End = new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false);

        internal static readonly ConsoleKeyInfo LeftArrow = new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false);
        internal static readonly ConsoleKeyInfo RightArrow = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
        internal static readonly ConsoleKeyInfo UpArrow = new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false);
        internal static readonly ConsoleKeyInfo DownArrow = new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false);

        internal static readonly ConsoleKeyInfo Tab = new ConsoleKeyInfo('\0', ConsoleKey.Tab, false, false, false);
        internal static readonly ConsoleKeyInfo ShiftTab = new ConsoleKeyInfo('\0', ConsoleKey.Tab, true, false, false);

        internal static readonly ConsoleKeyInfo ExclamationPoint = CharExtensions.ExclamationPoint.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo Space = CharExtensions.Space.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlA = CharExtensions.CtrlA.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlB = CharExtensions.CtrlB.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlD = CharExtensions.CtrlD.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlE = CharExtensions.CtrlE.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlF = CharExtensions.CtrlF.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlH = CharExtensions.CtrlH.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlK = CharExtensions.CtrlK.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlL = CharExtensions.CtrlL.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlN = CharExtensions.CtrlN.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlP = CharExtensions.CtrlP.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlT = CharExtensions.CtrlT.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlU = CharExtensions.CtrlU.ToConsoleKeyInfo();
        internal static readonly ConsoleKeyInfo CtrlW = CharExtensions.CtrlW.ToConsoleKeyInfo();
    }
}