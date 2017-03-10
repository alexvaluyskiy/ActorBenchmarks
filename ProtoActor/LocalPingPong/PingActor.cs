using System.Threading.Tasks;
using Proto;

namespace LocalPingPong
{
    public class PingActor : IActor
    {
        public class Msg
        {
            public Msg(PID sender)
            {
                Sender = sender;
            }

            public PID Sender { get; }
        }

        public class Start
        {
            public Start(PID sender)
            {
                Sender = sender;
            }

            public PID Sender { get; }
        }

        private readonly int _batchSize;
        private int _batch;
        private int _messageCount;
        private PID _replyTo;

        public PingActor(int messageCount, int batchSize)
        {
            _messageCount = messageCount;
            _batchSize = batchSize;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Start s:
                    SendBatch(context, s.Sender);
                    _replyTo = context.Sender;
                    break;
                case Msg m:
                    _batch--;

                    if (_batch > 0)
                    {
                        break;
                    }

                    if (!SendBatch(context, m.Sender))
                    {
                        _replyTo.Tell(true);
                    }
                    break;
            }
            return Actor.Done;
        }

        private bool SendBatch(IContext context, PID sender)
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

        public static Props Props(int messageCount, int batchSize) => Actor.FromProducer(() => new PingActor(messageCount, batchSize));
    }
}