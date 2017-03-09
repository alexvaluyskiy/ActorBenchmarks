using Akka.Actor;

namespace PongNode.Messages
{
    public class StartRemote
    {
        public StartRemote(IActorRef sender)
        {
            Sender = sender;
        }

        public IActorRef Sender { get; }
    }
}