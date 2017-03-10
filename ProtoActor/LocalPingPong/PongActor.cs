using System.Threading.Tasks;
using Proto;

namespace LocalPingPong
{
    public class PongActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case PingActor.Msg msg:
                    msg.Sender.Tell(msg);
                    break;
            }

            return Actor.Done;
        }

        public static Props Props => Actor.FromProducer(() => new PongActor());
    }
}