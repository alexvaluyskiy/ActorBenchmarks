using System.Threading.Tasks;
using Proto;

namespace SpawnBenchmark
{
    public sealed class SpawnActor : IActor
    {
        public class Start
        {
            public Start(int level, long number)
            {
                Level = level;
                Number = number;
            }

            public int Level { get; }

            public long Number { get; }
        }

        private int _todo = 10;
        private long _count = 0L;

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is Start start)
            {
                if (start.Level == 1)
                {
                    context.Parent.Tell(start.Number);
                    context.Self.Stop();
                }
                else
                {
                    var startNumber = start.Number * 10;

                    for (int i = 0; i <= 9; i++)
                    {
                        context.Spawn(Props).Tell(new Start(start.Level - 1, startNumber + i));
                    }
                }
                return Actor.Done;
            }
            else if (context.Message is long l)
            {
                _todo -= 1;
                _count += l;
                if (_todo == 0)
                {
                    context.Parent.Tell(_count);
                    //context.Self.Stop();
                }

                return Actor.Done;
            }

            return Actor.Done;
        }

        public static Props Props { get; } = Actor.FromProducer(() => new SpawnActor());
    }
}
