using Akka.Actor;
using MessagePack;

namespace LocalPingPong
{
    [MessagePackObject]
    [Bond.Schema]
    public sealed class Msg
    {
        public Msg()
        {
        }

        [SerializationConstructor]
        public Msg(string message)
        {
            Message = message;
        }

        [Key(0)]
        [Bond.Id(0)]
        public string Message { get; set; }
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
        private int _batch;
        private int _messageCount;
        private IActorRef _replyTo;
        private IActorRef _pongActor;

        public PingActor(int messageCount, int batchSize)
        {
            _messageCount = messageCount;
            _batchSize = batchSize;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Start s:
                    SendBatch(Context, s.Sender);
                    _replyTo = Sender;
                    _pongActor = s.Sender;
                    break;
                case Msg m:
                    _batch--;

                    if (_batch > 0)
                    {
                        break;
                    }

                    if (!SendBatch(Context, _pongActor))
                    {
                        _replyTo.Tell(true);
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

            var m = new Msg("ping");

            for (var i = 0; i < _batchSize; i++)
            {
                sender.Tell(m);
            }

            _messageCount -= _batchSize;
            _batch = _batchSize;
            return true;
        }

        public static Props Props(int messageCount, int batchSize)
        {
            return Akka.Actor.Props.Create(() => new PingActor(messageCount, batchSize));
        }
    }
}
