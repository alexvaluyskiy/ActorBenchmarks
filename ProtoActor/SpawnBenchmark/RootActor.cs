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

        private readonly Behavior _behavior;

        public RootActor()
        {
            _behavior = new Behavior();
        }

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is Run run)
            {
                StartRun(run.Number, context);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private void StartRun(int n, IContext context)
        {
            Console.WriteLine($"Start run {n}");

            var start = Stopwatch.ElapsedMilliseconds;
            context.Send(context.Spawn(Props.FromProducer(() => new SpawnActor())), new SpawnActor.Start(7, 0));
            _behavior.Become(Waiting(n - 1, start));
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
                        return Task.CompletedTask;
                    }
                    else
                    {
                        StartRun(n, context);
                    }

                    return Task.CompletedTask;
                }

                return Task.CompletedTask;
            };
        }
    }
}