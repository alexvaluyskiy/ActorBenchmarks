//-----------------------------------------------------------------------
// <copyright file="ClusterClientMessageSerializer.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Serialization;
using Google.Protobuf;

namespace LocalPingPong
{
    public class ProtobufSerializer : Serializer
    {
        public ProtobufSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override int Identifier { get; } = 50;

        public override bool IncludeManifest => true;

        public override byte[] ToBinary(object obj)
        {
            if (obj is LocalPingPong.Msg)
                return RemoteMessageBuilder((LocalPingPong.Msg)obj).ToByteArray();

            throw new ArgumentException($"Can't serialize object of type {obj.GetType()}");
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            if (type == typeof(LocalPingPong.Msg))
            {
                return RemoteMessageFrom(LocalPingPong.Protobuf.Msg.RemoteMessage.Parser.ParseFrom(bytes));
            }

            throw new ArgumentException(typeof(ProtobufSerializer) + " cannot deserialize object of type " + type);
        }

        private static LocalPingPong.Protobuf.Msg.RemoteMessage RemoteMessageBuilder(LocalPingPong.Msg remoteMessage)
        {
            var protoMessage = new LocalPingPong.Protobuf.Msg.RemoteMessage();
            protoMessage.Message = remoteMessage.Message;
            return protoMessage;
        }

        private static LocalPingPong.Msg RemoteMessageFrom(LocalPingPong.Protobuf.Msg.RemoteMessage protoMessage)
        {
            return new LocalPingPong.Msg(protoMessage.Message);
        }
    }
}