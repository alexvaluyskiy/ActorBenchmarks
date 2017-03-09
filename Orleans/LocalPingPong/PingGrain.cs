using Orleans;
using System.Threading.Tasks;

namespace LocalPingPong
{
    public interface IPingGrain : IGrainWithGuidKey
    {
        Task Run();
        Task Pong(IPongGrain from, Message message);
        Task Initialize(IPongGrain actor, long repeats);
        Task Subscribe(IClientObserver subscriber);
    }

    public class PingGrain : Grain, IPingGrain
    {
        static readonly Message msg = new Message();

        IPongGrain actor;
        ObserverSubscriptionManager<IClientObserver> subscribers;

        long pings;
        long pongs;
        long repeats;

        public override Task OnActivateAsync()
        {
            subscribers = new ObserverSubscriptionManager<IClientObserver>();
            return TaskDone.Done;
        }

        public Task Initialize(IPongGrain actor, long repeats)
        {
            this.actor = actor;
            this.repeats = repeats;

            return TaskDone.Done;
        }

        public Task Run()
        {
            actor.Ping(this, msg).Ignore();
            pings++;

            return TaskDone.Done;
        }

        public Task Pong(IPongGrain @from, Message message)
        {
            pongs++;

            if (pings < repeats)
            {
                actor.Ping(this, msg);
                pings++;
            }
            else if (pongs >= repeats)
            {
                subscribers.Notify(x => x.Done(pings, pongs));
            }

            return TaskDone.Done;
        }

        public Task Subscribe(IClientObserver subscriber)
        {
            subscribers.Subscribe(subscriber);
            return TaskDone.Done;
        }
    }
}
