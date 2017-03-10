using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Proto;

namespace SpawnBenchmark
{
    public sealed class RootActor : IActor
    {
        public class Run
        {
            public Run(int number)
            {
                Number = number;
            }

            public int Number { get; }
        }

        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is Run run)
            {
                StartRun(run.Number, context);

                return Actor.Done;
            }

            return Actor.Done;
        }

        private void StartRun(int n, IContext context)
        {
            Console.WriteLine($"Start run {n}");

            var start = Stopwatch.ElapsedMilliseconds;
            context.Spawn(SpawnActor.Props).Tell(new SpawnActor.Start(7, 0));
            context.SetBehavior(Waiting(n - 1, start));
        }

        private Receive Waiting(int n, long start)
        {
            return context =>
            {
                if (context.Message is long x)
                {
                    var diff = (Stopwatch.ElapsedMilliseconds - start);
                    Console.WriteLine($"Run {n + 1} result: {x} in {diff} ms");
                    if (n == 0)
                    {
                        return Actor.Done;
                    }
                    else
                    {
                        StartRun(n, context);
                    }

                    return Actor.Done;
                }

                return Actor.Done;
            };
        }

        public static Props Props { get; } = Actor.FromProducer(() => new RootActor());
    }
}