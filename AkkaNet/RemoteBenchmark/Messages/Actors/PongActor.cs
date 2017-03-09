using Akka.Actor;
using PongNode.Messages;
using System;

namespace PongNode.Actors
{
    public class PongActor : UntypedActor
    {
        private IActorRef _sender;

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartRemote sr:
                    Console.WriteLine($"Starting on {Context.Self.Path.ToString()}");
                    _sender = sr.Sender;
                    Sender.Tell(new Start());
                    break;
                case Ping _:
                    _sender.Tell(new Pong());
                    break;
            }
        }
    }
}

