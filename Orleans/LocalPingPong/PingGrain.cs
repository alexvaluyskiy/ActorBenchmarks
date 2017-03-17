using Orleans;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

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
        private ObserverSubscriptionManager<IBenchmarkObserver> subscribers = new ObserverSubscriptionManager<IBenchmarkObserver>();

        public Task Init(IPongGrain pongGrain, int messageCount, int batchSize)
        {
            _messageCount = messageCount;
            _batchSize = batchSize;
            _pongGrain = pongGrain;

            return TaskDone.Done;
        }

        public Task Start()
        {
            SendBatch(_pongGrain);

            return TaskDone.Done;
        }

        public Task Pong(IPongGrain pongGrain, Message message)
        {
            _batch--;

            if (_batch > 0)
            {
                return TaskDone.Done;
            }

            if (!SendBatch(pongGrain))
            {
                subscribers.Notify(s => s.BenchmarkFinished());
            }

            return TaskDone.Done;
        }

        public Task Subscribe(IBenchmarkObserver observer)
        {
            subscribers.Subscribe(observer);

            return TaskDone.Done;
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
