using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XdChatShared.Misc;

/* => Packet format <=
 *
 * => Packets are formatted in json
 * => Every packet has a 'type' field. It contains the class name of the packet
 * => Every packet has a 'data' field. It contains the fields of a packet
 *
 * => All packets extend Packet
 * => Packets starting with "ServerPacket" are sent by the server
 * => Packets starting with "ClientPacket" are sent by the client
 */
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
        
        public abstract string Validate();
    }
}