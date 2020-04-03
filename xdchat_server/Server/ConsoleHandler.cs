using System;
using System.Collections.Generic;
using SimpleLogger;
using SimpleLogger.Logging;
using XdChatShared.Scheduler;

namespace xdchat_server {
    public class ConsoleHandler {
        private readonly List<string> _lineBuffer = new List<string>();
        private readonly Action<string> _inputHandler;
        private bool _readMode, _running = true;

        public ConsoleHandler(Action<string> inputHandler) {
            this._inputHandler = inputHandler;

            XdScheduler.QueueAsyncTask(RunReadTask, true);

            Logger.LoggerHandlerManager.AddHandler(new LoggingHandler(this));
        }

        private void RunReadTask() {
            while (_running) {
                ConsoleKeyInfo input = Console.ReadKey();
                if (char.IsControl(input.KeyChar)) continue;
                
                SetReadMode(true);
                string line = Console.ReadLine();
                SetReadMode(false);
                
                if (line != null && _running) {
                    _inputHandler.Invoke(input.KeyChar + line);
                }
            }
        }

        private void SetReadMode(bool val) {
            lock (this) {
                if (val) {
                    _readMode = true;
                    return;
                }
                
                _lineBuffer.ForEach(Console.WriteLine);
                _lineBuffer.Clear();
                _readMode = false;
            }
        }

        public void WriteLine(string line) {
            lock (this) {
                if (_readMode) {
                    if (_lineBuffer.Count < 5000) { // Prevent out of memory
                        _lineBuffer.Add(line);
                    }
                } else {
                    Console.WriteLine(line);
                }
            }
        }

        public void Stop() {
            _running = false;
            
            SetReadMode(false);
        }
    }

    class LoggingHandler : ILoggerHandler {
        private readonly ConsoleHandler _handler;

        public LoggingHandler(ConsoleHandler handler) {
            this._handler = handler;
        }

        public void Publish(LogMessage logMessage) {
            _handler.WriteLine($"{(object) logMessage.DateTime:dd.MM.yyyy HH:mm}: {(object) logMessage.Level} " +
                              $"[line: {(object) logMessage.LineNumber} {(object) logMessage.CallingClass} ->" +
                              $" {(object) logMessage.CallingMethod}()]: {(object) logMessage.Text}");
        }
    }
}