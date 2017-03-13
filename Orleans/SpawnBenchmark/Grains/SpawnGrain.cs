using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpawnBenchmark
{
    public interface ISpawnGrain : IGrainWithStringKey
    {
        Task Initialize(IRootGrain root);

        Task Start(ISpawnGrain sender, int level, long number);

        Task Update(long number);
    }

    public class SpawnGrain : Grain, ISpawnGrain
    {
        private const int childCount = 9;

        private int _todo = childCount;
        private long _count = 0L;

        private ISpawnGrain _sender;

        private IRootGrain _root;

        public Task Initialize(IRootGrain root)
        {
            _root = root;

            return Task.CompletedTask;
        }

        public Task Start(ISpawnGrain sender, int level, long number)
        {
            _sender = sender;

            if (level == 1)
            {
                _sender.Update(number).Ignore();
            }
            else
            {
                var startNumber = number * childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var childGrain = this.GrainFactory.GetGrain<ISpawnGrain>($"Grain<{level - 1}, {startNumber + i}>");
                    childGrain.Start(this, level - 1, startNumber + i).Ignore();
                }
            }

            return Task.CompletedTask;
        }

        public Task Update(long number)
        {
            _todo -= 1;
            _count += number;
            if (_todo == 0)
            {
                if (_sender == null && _root != null)
                {
                    _root.Complete(_count).Ignore();
                }
                else
                {
                    _sender.Update(_count).Ignore();
                }
                
            }

            return Task.CompletedTask;
        }
    }
}
