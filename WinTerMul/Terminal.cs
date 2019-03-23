﻿using System.Diagnostics;

using PInvoke;

using WinTerMul.Common;

using static WinTerMul.Common.NativeMethods;

namespace WinTerMul
{
    internal class Terminal
    {
        public Process Process { get; private set; }
        public Pipe Out { get; private set; }
        public Pipe In { get; private set; }

        public COORD CursorPosition { get; set; }
        public CONSOLE_CURSOR_INFO CursorInfo { get; set; }

        public static Terminal Create()
        {
            var terminal = new Terminal
            {
                Out = Pipe.Create(), // TODO make sure to dispose pipes
                In = Pipe.Create() 
            };

            terminal.Process = new Process
            {
                // TODO change path
                StartInfo = new ProcessStartInfo(@"C:\Users\zalewski\source\repos\WinTerMul\WinTerMul.Terminal\bin\Debug\net461\WinTerMul.Terminal.exe")
                {
                    Arguments = $"{terminal.Out.Id} {terminal.In.Id} {Process.GetCurrentProcess().Id}"
                }
            };
            terminal.Process.Start();

            return terminal;
        }
    }
}
