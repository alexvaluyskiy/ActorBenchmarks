using Proto;
using System.Threading.Tasks;

namespace SpawnBenchmarkNoChildren
{
    public class SpawnActor : IActor
    {
        private long _replies;
        private PID _replyTo;
        private long _sum;

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is SpawnRequest r)
            {
                if (r.Size == 1)
                {
                    context.Respond(r.Num);
                    context.Self.Stop();
                    return Actor.Done;
                }
                _replies = r.Div;
                _replyTo = context.Sender;
                for (var i = 0; i < r.Div; i++)
                {
                    var child = Actor.Spawn(Props);
                    child.Request(new SpawnRequest
                    (
                        num: r.Num + i * (r.Size / r.Div),
                        size: r.Size / r.Div,
                        div: r.Div
                    ), context.Self);
                }

                return Actor.Done;
            }
            else if (context.Message is long l)
            {
                _sum += l;
                _replies--;
                if (_replies == 0)
                {
                    _replyTo.Tell(_sum);
                }
                return Actor.Done;
            }

            return Actor.Done;
        }

        public static Props Props { get; } = Actor.FromProducer(() => new SpawnActor());
    }
}