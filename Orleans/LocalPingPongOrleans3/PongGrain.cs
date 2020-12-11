using Orleans;
using System.Threading.Tasks;

namespace LocalPingPong
{
    public interface IPongGrain : IGrainWithIntegerKey
    {
        Task Ping(IPingGrain pingGrain, Message message);
    }

    public class PongGrain : Grain, IPongGrain
    {
        public Task Ping(IPingGrain pingGrain, Message message)
        {
            pingGrain.Pong(this, message).Ignore();
            return Task.CompletedTask;
        }
    }
}
