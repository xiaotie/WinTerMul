﻿using System;

using WinTerMul.Common;
using WinTerMul.Common.Kernel32;

namespace WinTerMul
{
    internal class InputHandler : IDisposable
    {
        private readonly TerminalContainer _terminalContainer;
        private readonly IKernel32Api _kernel32Api;

        private bool _wasLastKeyCtrlK;

        public InputHandler(TerminalContainer terminalContainer, IKernel32Api kernel32Api)
        {
            _terminalContainer = terminalContainer;
            _kernel32Api = kernel32Api;
        }

        public void HandleInput()
        {
            var inputRecord = _kernel32Api.ReadConsoleInput();
            if (inputRecord.EventType == InputEventTypeFlag.KeyEvent)
            {
                if (_wasLastKeyCtrlK)
                {
                    _wasLastKeyCtrlK = false;

                    switch (inputRecord.Event.KeyEvent.Char.UnicodeChar)
                    {
                        case 'k':
                        case '\v':
                        case '\0':
                        case '\u000f':
                            _wasLastKeyCtrlK = true;
                            break;
                        case 'l':
                            _terminalContainer.SetNextTerminalActive();
                            break;
                        case 'h':
                            _terminalContainer.SetPreviousTerminalActive();
                            break;
                        case 'v':
                            _terminalContainer.AddTerminal(Terminal.Create());
                            break;
                        //case 'h': // TODO horizontal split
                        default:
                            break;
                    }

                    return;
                }

                if (inputRecord.Event.KeyEvent.Char.UnicodeChar == '\v') // CTRL + k
                {
                    _wasLastKeyCtrlK = true;
                    return;
                }

                try
                {
                    _terminalContainer.ActiveTerminal?.In.Write(new TransferableInputRecord { InputRecord = inputRecord });
                }
                catch (ObjectDisposedException)
                {
                    // Process has exited, new active terminal will be set in next iteration.
                    return;
                }
            }
        }

        public void Dispose()
        {
            _terminalContainer?.Dispose();
        }
    }
}
