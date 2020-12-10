using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalPingPong
{
    public interface IPingGrain : IGrainWithIntegerKey
    {
        Task Init(IPongGrain pongGrain, int messageCount, int batchSize);

        Task Start();

        Task Pong(IPongGrain pongGrain, Message message);

        Task Subscribe(IBenchmarkObserver observer);
    }

    public class PingGrain : Grain, IPingGrain
    {
        private int _messageCount;
        private int _batchSize;
        private int _batch;
        private IPongGrain _pongGrain;
        private IBenchmarkObserver _benchmarkObserver;

        public Task Init(IPongGrain pongGrain, int messageCount, int batchSize)
        {
            _messageCount = messageCount;
            _batchSize = batchSize;
            _pongGrain = pongGrain;

            return Task.CompletedTask;
        }

        public Task Start()
        {
            SendBatch(_pongGrain);

            return Task.CompletedTask;
        }

        public Task Pong(IPongGrain pongGrain, Message message)
        {
            _batch--;

            if (_batch > 0)
            {
                return Task.CompletedTask;
            }

            if (!SendBatch(pongGrain))
            {
                _benchmarkObserver.BenchmarkFinished();
            }

            return Task.CompletedTask;
        }

        public Task Subscribe(IBenchmarkObserver observer)
        {
            _benchmarkObserver = observer;

            return Task.CompletedTask;
        }

        private bool SendBatch(IPongGrain pongGrain)
        {
            if (_messageCount == 0)
            {
                return false;
            }

            var message = new Message();

            for (var i = 0; i < _batchSize; i++)
            {
                pongGrain.Ping(this, message);
            }

            _messageCount -= _batchSize;
            _batch = _batchSize;
            return true;
        }
    }
}
