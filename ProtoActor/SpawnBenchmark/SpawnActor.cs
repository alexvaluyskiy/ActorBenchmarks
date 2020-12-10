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
                    context.Send(context.Parent, start.Number);
                    context.Stop(context.Self);
                }
                else
                {
                    var startNumber = start.Number * 10;

                    for (int i = 0; i <= 9; i++)
                    {
                        context.Send(
                            context.Spawn(Props.FromProducer(() => new SpawnActor())),
                            new Start(start.Level - 1, startNumber + i));
                    }
                }
                return Task.CompletedTask;
            }
            else if (context.Message is long l)
            {
                _todo -= 1;
                _count += l;
                if (_todo == 0)
                {
                    context.Send(context.Parent, _count);
                    //context.Stop(context.Self);
                }

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
