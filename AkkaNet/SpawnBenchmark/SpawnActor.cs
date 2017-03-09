using Akka.Actor;

namespace SpawnBenchmark
{
    internal class SkynetFast : UntypedActor
    {
        private long replies;
        private IActorRef replyTo;
        private long sum = 0;

        protected override void OnReceive(object message)
        {
            if (message is long l)
            {
                sum += l;
                replies--;
                if (replies == 0)
                {
                    replyTo.Tell(sum);
                }
            }
            if (message is SpawnRequest msg)
            {
                if (msg.Size == 1)
                {
                    Sender.Tell(msg.Num);
                    Context.Stop(Context.Self);
                    return;
                }

                replyTo = Sender;
                replies = msg.Div;
                for (var i = 0; i < msg.Div; i++)
                {
                    var child = Context.ActorOf(Props);
                    child.Tell(new SpawnRequest
                    (
                        num: msg.Num + i * (msg.Size / msg.Div),
                        size: msg.Size / msg.Div,
                        div: msg.Div
                    ), Self);
                }
            }
        }

        public static Props Props { get; } = Props.Create<SkynetFast>();
    }
}
