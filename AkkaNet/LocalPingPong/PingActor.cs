using Akka.Actor;
using System.Threading.Tasks;

namespace LocalPingPong
{
    public class Msg
    {
        public Msg(IActorRef sender)
        {
            Sender = sender;
        }

        public IActorRef Sender { get; }
    }

    public class Start
    {
        public Start(IActorRef sender)
        {
            Sender = sender;
        }

        public IActorRef Sender { get; }
    }

    public class PingActor : UntypedActor
    {
        private readonly int _batchSize;
        private readonly TaskCompletionSource<bool> _wgStop;
        private int _batch;
        private int _messageCount;

        public PingActor(TaskCompletionSource<bool> wgStop, int messageCount, int batchSize)
        {
            _wgStop = wgStop;
            _messageCount = messageCount;
            _batchSize = batchSize;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Start s:
                    SendBatch(Context, s.Sender);
                    break;
                case Msg m:
                    _batch--;

                    if (_batch > 0)
                    {
                        break;
                    }

                    if (!SendBatch(Context, m.Sender))
                    {
                        _wgStop.SetResult(true);
                    }
                    break;
            }
        }

        private bool SendBatch(IActorContext context, IActorRef sender)
        {
            if (_messageCount == 0)
            {
                return false;
            }

            var m = new Msg(context.Self);

            for (var i = 0; i < _batchSize; i++)
            {
                sender.Tell(m);
            }

            _messageCount -= _batchSize;
            _batch = _batchSize;
            return true;
        }

        public static Props Props(TaskCompletionSource<bool> wgStop, int messageCount, int batchSize)
        {
            return Akka.Actor.Props.Create(() => new PingActor(wgStop, messageCount, batchSize));
        }
    }
}
