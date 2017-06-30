using System;
using System.IO;
using Akka.Actor;
using Hyperion;
using Serializer = Akka.Serialization.Serializer;

namespace LocalPingPong.Serializers
{
    public class HyperionSerializer : Serializer
    {
        private readonly Hyperion.Serializer _serializer;

        public HyperionSerializer(ExtendedActorSystem system): base(system)
        {
            var knownTypes = new[] { typeof(Msg) };
            var serializerOptions = new SerializerOptions(knownTypes: knownTypes);
            _serializer = new Hyperion.Serializer(serializerOptions);
        }

        public override int Identifier => 52;

        public override bool IncludeManifest => false;

        public override byte[] ToBinary(object obj)
        {
            using (var ms = new MemoryStream())
            {
                _serializer.Serialize(obj, ms);
                return ms.ToArray();
            }
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var res = _serializer.Deserialize<object>(ms);
                return res;
            }
        }
    }
}
