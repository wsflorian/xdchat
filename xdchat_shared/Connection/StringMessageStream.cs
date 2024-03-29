﻿using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace XdChatShared.Connection {
    /* => String message protocol <=
     * 
     * Message format
     * => length (4 byte - uint): Contains the length (n) of 'data'
     * => data (n byte): Contains the string encoded as utf8 
     */
    public class StringMessageStream : IDisposable {
        private readonly Stream _stream;
        
        public StringMessageStream([NotNull] Stream stream) {
            this._stream = stream;
        }

        public void WriteMessage([NotNull] string message) {
            byte[] stringData = Encoding.UTF8.GetBytes(message);
            byte[] lengthData = BitConverter.GetBytes(stringData.Length);
            
            _stream.Write(lengthData, 0, lengthData.Length);
            _stream.Write(stringData, 0, stringData.Length);
            _stream.Flush();
        }
        
        public string ReadMessage() {
            byte[] lengthData = ReadSurely(4);
            uint length = BitConverter.ToUInt32(lengthData, 0);

            return Encoding.UTF8.GetString(ReadSurely(length));
        }

        private byte[] ReadSurely(uint shallRead) {
            byte[] readBuf = new byte[shallRead];
            int totalRead = 0;

            while (totalRead < shallRead) {
                int amountRead = _stream.Read(readBuf, totalRead, (int) (shallRead - totalRead));
                if (amountRead == 0) {
                    throw new EndOfStreamException();
                }
                
                totalRead += amountRead;
            }

            return readBuf;
        }

        public void Dispose() {
            _stream?.Dispose();
        }
    }
}