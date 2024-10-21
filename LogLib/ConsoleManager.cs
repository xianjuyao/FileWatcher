using System;
using System.Runtime.InteropServices;


namespace LogLib
{
    internal class ConsoleManager
    {
        private const string DllName = "kernel32.dll";

        [DllImport(DllName)]
        private static extern bool AllocConsole();

        [DllImport(DllName)]
        private static extern bool FreeConsole();

        [DllImport(DllName)]
        private static extern IntPtr GetConsoleWindow();

        private static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

        // Creates a new console instance
        // if the process is not attached to a console already.
        public void ShowConsole()
        {
            if (!HasConsole) AllocConsole();
        }
        //隐藏窗口
        public void HideConsole()
        {
            if (HasConsole) FreeConsole();
        }
    }
}