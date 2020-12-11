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
                    context.Send(msg.Sender, msg);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}