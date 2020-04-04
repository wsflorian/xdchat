using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XdChatShared.Misc;

namespace XdChatShared.Packets {
    public abstract class Packet : IValidatable {
        public bool IsType(Type type) {
            return this.GetType() == type;
        }

        [NotNull]
        public static string ToJson([NotNull]Packet data) {
            return new JObject {
                {"type", data.GetType().Name}, 
                {"data", JToken.FromObject(data)}
            }.ToString();
        }

        [NotNull] 
        public static Packet FromJson([NotNull] string message) {
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
                throw new ProtocolException($"Json is invalid: {e.Message}: '{message}'");
            }
        }
        
        [CanBeNull]
        public abstract string Validate();
    }
}