using System;
using System.Collections.Concurrent;
using Bond.IO.Unsafe;
using Bond.Protocols;

namespace LocalPingPong.Serializers
{
    using global::Bond;
    using System.Linq;

    /// <summary>
    /// Microsoft Bond serializer compatible with Akka.NET API.
    /// </summary>
    public sealed class BondSerializer : Akka.Serialization.Serializer
    {
        private readonly ConcurrentDictionary<Type, Serializer<FastBinaryWriter<OutputBuffer>>> serializerCache = new ConcurrentDictionary<Type, Serializer<FastBinaryWriter<OutputBuffer>>>();
        private readonly ConcurrentDictionary<Type, Deserializer<FastBinaryReader<InputBuffer>>> deserializerCache = new ConcurrentDictionary<Type, Deserializer<FastBinaryReader<InputBuffer>>>();

        public BondSerializer(Akka.Actor.ExtendedActorSystem system) : base(system)
        {
        }

        public override int Identifier { get; } = 120;
        public override bool IncludeManifest => false;
        
        public override byte[] ToBinary(object obj)
        {
            
            var serializer = serializerCache.GetOrAdd(obj.GetType(), t => new Serializer<FastBinaryWriter<OutputBuffer>>(t));
            OutputBuffer outputBuffer = new OutputBuffer(1024);
            var writer = new FastBinaryWriter<OutputBuffer>(outputBuffer);
            serializer.Serialize(obj, writer);

            int length = (int)(outputBuffer.Position - outputBuffer.Data.Offset);
            var serialized = new byte[length];
            Array.Copy(outputBuffer.Data.Array, outputBuffer.Data.Offset, serialized, 0, length);
            return serialized;
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            type = type ?? typeof(Msg);
            if (type == null) throw new InvalidOperationException($"{GetType()}.FromBinary requires type to be provided");

            var deserializer = deserializerCache.GetOrAdd(type, t => new Deserializer<FastBinaryReader<InputBuffer>>(t));
            var inputBuffer = new InputBuffer(bytes);
            var reader = new FastBinaryReader<InputBuffer>(inputBuffer);
            var obj = deserializer.Deserialize(reader);
            return obj;
        }
    }
}