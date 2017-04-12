using System;
using Akka.Actor;
using Akka.Serialization;
using Google.Protobuf;

namespace LocalPingPong.Serializers
{
    public class ProtobufSerializer : Serializer
    {
        public ProtobufSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override int Identifier { get; } = 50;

        public override bool IncludeManifest => false;

        public override byte[] ToBinary(object obj)
        {
            if (obj is Msg)
                return RemoteMessageBuilder((Msg)obj);

            throw new ArgumentException($"Can't serialize object of type {obj.GetType()}");
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            if (type == typeof(Msg))
            {
                return RemoteMessageFrom(bytes);
            }

            throw new ArgumentException(typeof(ProtobufSerializer) + " cannot deserialize object of type " + type);
        }

        private static byte[] RemoteMessageBuilder(Msg remoteMessage)
        {
            var protoMessage = new Protobuf.Msg.RemoteMessage {Message = remoteMessage.Message};
            return protoMessage.ToByteArray();
        }

        private static Msg RemoteMessageFrom(byte[] bytes)
        {
            var protoMessage = Protobuf.Msg.RemoteMessage.Parser.ParseFrom(bytes);
            return new Msg(protoMessage.Message);
        }
    }
}