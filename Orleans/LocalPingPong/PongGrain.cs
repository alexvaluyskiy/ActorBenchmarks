using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalPingPong
{
    public interface IPongGrain : IGrainWithGuidKey
    {
        Task Ping(IPingGrain from, Message message);
    }

    public class PongGrain : Grain, IPongGrain
    {
        public Task Ping(IPingGrain @from, Message message)
        {
            from.Pong(this, message).Ignore();
            return TaskDone.Done;
        }
    }
}
