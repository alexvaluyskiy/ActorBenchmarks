using Proto;
using System;
using System.Threading.Tasks;

namespace SpawnBenchmark
{
    internal class SpawnActor : IActor
    {
        private long Replies;
        private PID ReplyTo;
        private long Sum;

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            var r = msg as SpawnRequest;
            if (r != null)
            {
                if (r.Size == 1)
                {
                    context.Respond(r.Num);
                    context.Self.Stop();
                    return Actor.Done;
                }
                Replies = r.Div;
                ReplyTo = context.Sender;
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
            if (msg is Int64)
            {
                Sum += (Int64)msg;
                Replies--;
                if (Replies == 0)
                {
                    ReplyTo.Tell(Sum);
                }
                return Actor.Done;
            }
            return Actor.Done;
        }

        public static Props Props { get; } = Actor.FromProducer(() => new SpawnActor());
    }
}