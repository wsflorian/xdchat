using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XdChatShared.Packets {
    public abstract class Packet : IValidatable {
        public bool IsType(Type type) {
            return this.GetType() == type;
        }

        public static string ToJson(Packet data) {
            JObject packetObj = new JObject {
                {"type", data.GetType().Name}, 
                {"data", JToken.FromObject(data)}
            };
            
            return packetObj.ToString();
        }

        public static Packet FromJson(string message) {
            try {
                JObject packetObj = JObject.Parse(message);
                JToken packetTypeToken = packetObj.GetValue("type");

                if (packetTypeToken == null || packetTypeToken.Type != JTokenType.String) {
                    throw new ProtocolException("'type' is not string");
                }

                Type packetType = Type.GetType( $"{typeof(Packet).Namespace}.{packetTypeToken}");
                if (packetType == null) {
                    throw new ProtocolException($"Packet type '{packetTypeToken}' not found");
                }

                Packet packet = (Packet) packetObj.GetValue("data").ToObject(packetType);

                string validationError = packet.Validate();
                if (validationError != null) {
                    throw new ProtocolException($"Packet validation failed for type {packetType}: {validationError}");
                }

                return packet;
            } catch (JsonReaderException e) {
                throw new ProtocolException($"Json is invalid: {e.Message}");
            }
        }

        public static void InvokeActionIfType<T>(Packet packet, Action<T> action) where T: Packet {
            if (packet.IsType(typeof(T))) {
                action.Invoke((T) packet); 
            }
        }

        public abstract string Validate();
    }
    
    public class ProtocolException : Exception {
        public ProtocolException(string message) : base(message) { }
    }
}