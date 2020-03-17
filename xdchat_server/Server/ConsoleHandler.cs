using System;
using System.Collections.Generic;
using SimpleLogger;
using SimpleLogger.Logging;
using XdChatShared.Scheduler;

namespace xdchat_server {
    public class ConsoleHandler {
        private readonly List<string> lineBuffer = new List<string>();
        private readonly Action<string> inputHandler;
        private bool readMode, running = true;

        public ConsoleHandler(Action<string> inputHandler) {
            this.inputHandler = inputHandler;

            XdScheduler.QueueAsyncTask(RunReadTask, true);

            Logger.LoggerHandlerManager.AddHandler(new LoggingHandler(this));
        }

        private void RunReadTask() {
            while (running) {
                ConsoleKeyInfo input = Console.ReadKey();
                if (char.IsControl(input.KeyChar)) continue;
                
                SetReadMode(true);
                string line = Console.ReadLine();
                SetReadMode(false);
                
                if (line != null && running) {
                    inputHandler.Invoke(input.KeyChar + line);
                }
            }
        }

        private void SetReadMode(bool val) {
            lock (this) {
                if (val) {
                    readMode = true;
                    return;
                }
                
                lineBuffer.ForEach(Console.WriteLine);
                lineBuffer.Clear();
                readMode = false;
            }
        }

        public void WriteLine(string line) {
            lock (this) {
                if (readMode) {
                    if (lineBuffer.Count < 5000) { // Prevent out of memory
                        lineBuffer.Add(line);
                    }
                } else {
                    Console.WriteLine(line);
                }
            }
        }

        public void Stop() {
            running = false;
            
            SetReadMode(false);
        }
    }

    class LoggingHandler : ILoggerHandler {
        private readonly ConsoleHandler handler;

        public LoggingHandler(ConsoleHandler handler) {
            this.handler = handler;
        }

        public void Publish(LogMessage logMessage) {
            handler.WriteLine($"{(object) logMessage.DateTime:dd.MM.yyyy HH:mm}: {(object) logMessage.Level} " +
                              $"[line: {(object) logMessage.LineNumber} {(object) logMessage.CallingClass} ->" +
                              $" {(object) logMessage.CallingMethod}()]: {(object) logMessage.Text}");
        }
    }
}