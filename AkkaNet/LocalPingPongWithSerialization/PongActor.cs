using Akka.Actor;

namespace LocalPingPong
{
    public class PongActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Sender.Tell(message);
        }

        public static Props Props { get; } = Props.Create<PongActor>();
    }
}
