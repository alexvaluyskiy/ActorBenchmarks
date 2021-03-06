using System;
using Akka.Actor;
using Akka.Serialization;
using MessagePack;

namespace LocalPingPong.Serializers
{
    public class MsgPackSerializer : Serializer
    {
        public MsgPackSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override byte[] ToBinary(object obj)
        {
            return MessagePackSerializer.NonGeneric.Serialize(obj.GetType(), obj);
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            return MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
        }

        public override int Identifier => 51;

        public override bool IncludeManifest => false;
    }
}