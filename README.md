# Performance and Benchmarks

## Benchmarks
### LocalPingPong
There is a single node and there may be more than two actors, it runs for 1, 2, 4, 8, 12 actors. Messages may or may not be serialized. The test then pass 1 mil messages from `PingActor` to `PongActor` and back again. There is no specific message size taken into account here, the message may be as small as your framework supports.

### SpawnBenchmark
Creates an actor, which spawns 10 new actors, each of them spawns 10 more actors, etc. until one million actors are created on the final level. Then, each of them returns back its ordinal number (from 0 to 999999), which are summed on the previous level and sent back upstream, until reaching the root actor. (The answer should be 499999500000).
https://github.com/atemerev/skynet

## Results
- Hardware: Windown 10, Intel Core i5 3570, 8 GB RAM
- Software: .NET 5.0.1, Go 1.8, Scala 2.12
- Frameworks: Akka.NET 1.4.12, Orleans 3.3.0, ProtoActor 0.1.2, Akka 2.4.17

### SpawnBenchmark
|Library	                  | Platform            | Result             |
|---                          |---                  |---                 |
|ProtoActor (No Children)	  | .NET	            | 1.29 sec           |
|ProtoActor (No Children)     | Go                  | 2.24 sec           |
|ProtoActor                   | .NET	            | 2.71 sec           |
|Akka                         | Scala               | 6.39 sec           |
|Akka.NET                     | .NET                | 9.64 sec           |
|ProtoActor                   | Go                  | -                  |
|Orleans                      | .NET                | FAILED             |

### LocalPingPong (8 actors on both sides)
|Library	                  | Platform            | Result             |
|---                          |---                  |---                 |
|ProtoActor                   | Go                  | ~ 77 mil msg/s     |
|ProtoActor                   | .NET	            | ~ 64 mil msg/s     |
|Akka.NET                     | .NET                | ~ 43 mil msg/s     |
|Akka.NET (serialization)     | .NET                | ~ 174000 msg/s     |
|Orleans                      | .NET                | ~ 170000 msg/s     |
|Akka                         | Scala               | -                  |

### RemoteBenchmark (1 actor on both sides)
|Library	                  | Platform            | Result             |
|---                          |---                  |---                 |
|ProtoActor                   | Go                  | 2.6 mil msg/s      |
|ProtoActor                   | .NET	            | 2.4 mil msg/s      |
|Akka.NET                     | .NET                | 38000 msg/s        |
|Akka                         | Scala               | -                  |
|Akka (Artery)                | Scala               | -                  |
|Orleans                      | .NET                | -                  |

## How to run
### Run C# benchmarks
1. Install .NET SDK 5.0.0
2. Go to the benchmack folder
3. Run `dotnet restore` and `dotnet build -c Release`
4. Go to the folder '/bin/Release/net462' and run an executable file

For `ProtoActor` you also could run a NetCore version of benchmark, just type `dotnet run -c Release`

### Scala examples
1. Install Sbt 0.13.13
2. Run `sbt compile run`
