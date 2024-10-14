using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogLib
{
    internal class ConsoleManager
    {
        private const string dllName = "kernel32.dll";
        [DllImport(dllName)]
        private static extern bool AllocConsole();
        [DllImport(dllName)]
        private static extern bool FreeConsole();
        [DllImport(dllName)]
        private static extern IntPtr GetConsoleWindow();
        public static bool HasConsole{
            get {
                return GetConsoleWindow() != IntPtr.Zero;
            }
        }

        // Creates a new console instance
        // if the process is not attached to a console already.
        public void ShowConsole() {
            if (!HasConsole) AllocConsole();
        }
        public void HideConsole() { 
            if(HasConsole)FreeConsole();
        }
        public ConsoleManager() { 

        }
    }
}
