# Performance and Benchmarks

## Benchmarks
### LocalPingPong
There is a single node and there may be more than two actors, it runs for 1, 2, 4, 8, 16 actors. Messages may or may not be serialized. The test then pass 1 mil messages from `PingActor` to `PongActor` and back again. There is no specific message size taken into account here, the message may be as small as your framework supports.

### SpawnBenchmark
Creates an actor, which spawns 10 new actors, each of them spawns 10 more actors, etc. until one million actors are created on the final level. Then, each of them returns back its ordinal number (from 0 to 999999), which are summed on the previous level and sent back upstream, until reaching the root actor. (The answer should be 499999500000).
https://github.com/atemerev/skynet

## Results
- Hardware: Windown 10, Intel Core i5 3570, 8 GB RAM
- Software: .NET 4.6.2, NetCore CLI 1.0, NetCore SDK 1.1.

|Lib	                | LocalPingPong         | RemoteBenchmark    | SpawnBenchmark  |
|---                    |---                    |---                 |---              |
|Akka.NET 1.1.3	        | ~ 43 mil msg/s	    | 38000 msg/s        | 9.64 sec        |
|ProtoActor C# 0.1.1    | ~ 61 mil msg/s	    | 2.4 mil msg/s      | 1.29 sec        |
|Orleans 1.4.0          | -                     | -                  | -               |
|Akka 2.4.17            | -                     | -                  | -               |


## How to run
### C# examples
1. Install .NET Core SDK 1.1.1
2. Go to the benchmack folder
3. Run `dotnet restore` and `dotnet build -c Release`
4. Go to the folder '/bin/Release/net462' and run an executable file

For `ProtoActor` you also could run a NetCore version of benchmark, just type `dotnet run -c Release`

### Scala examples
1. TBD
