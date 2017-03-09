using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalPingPong
{
    [Immutable]
    public class Message { }

    public interface IClientObserver : IGrainObserver
    {
        void Done(long pings, long pongs);
    }

}
