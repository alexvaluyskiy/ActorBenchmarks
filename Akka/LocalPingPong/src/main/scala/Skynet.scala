import akka.actor.{Actor, ActorContext, ActorRef, ActorSystem, Props}

object PongActor {
  val props = Props(new PongActor)
}

class PongActor extends Actor {
  override def receive: Receive = {
    case msg =>
      sender() ! msg
  }
}

object PingActor {
  def props(wgStop: AnyRef, messageCount: Int, batchSize: Int) = Props(new PingActor(wgStop, messageCount, batchSize))

  case class Msg(sender: ActorRef)
  case class Start(sender: ActorRef)
}

class PingActor(wgStop: AnyRef, var messageCount: Int, batchSize: Int) extends Actor {
  import PingActor._

  var batch = 0

  override def receive: Receive = {
    case s: Start =>
      SendBatch(context, s.sender)
    case m: Msg =>
      batch = batch - 1

      if (batch > 0) {
        // return;
      }

      if (!SendBatch(context, m.sender)) {
        // wgStop.SetResult(true);
      }
  }

  def SendBatch(actorContext: ActorContext, actorRef: ActorRef): Boolean =  {
    if (messageCount == 0) {
      return false
    }

    var m = Msg(context.self)

    for (_ <- 1 to batchSize) {
      sender() ! m
    }

    messageCount = messageCount - batchSize
    batch = batchSize

    return true
  }
}

object Root extends App {
  case class Run(num: Int)

  val system = ActorSystem("main")
  val messageCount = 1000000
  val batchSize = 100
  val clientsCount = List(1, 2, 4, 8, 16)

  val clients = clientsCount.map(c => system.actorOf(PingActor.props(Nil, messageCount, batchSize))).toIndexedSeq
  var echos = clientsCount.map(c => system.actorOf(PongActor.props)).toIndexedSeq
}
