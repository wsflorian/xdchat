using System;
using System.Collections.Generic;
using xdchat_server.EventsImpl;
using xdchat_shared.Logger.Impl;
using XdChatShared.Logger;
using XdChatShared.Scheduler;

namespace xdchat_server.Server {
    public class ConsoleHandler {
        private readonly List<string> _lineBuffer = new List<string>();
        private bool _readMode, _running = true;

        public ConsoleHandler() {
            XdScheduler.QueueAsyncTask(RunReadTask, true);
            XdLogger.Instance.Publishers.Add(new ServerLogPublisher(this));
        }

        private void RunReadTask() {
            while (_running) {
                ConsoleKeyInfo input = Console.ReadKey();
                if (char.IsControl(input.KeyChar)) continue;
                
                SetReadMode(true);
                string line = Console.ReadLine();
                SetReadMode(false);
                
                if (line != null && _running) {
                    XdScheduler.QueueSyncTask(() => {
                        XdServer.Instance.EventEmitter.Emit(new ConsoleInputEvent(input.KeyChar + line));
                    });
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
}