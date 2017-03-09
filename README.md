# Performance and Benchmarks

## Results

|Lib	                  | LocalPingPong         | SpawnBenchmark  |
|---                    |---                    |---              |
|Akka.NET 1.1.3	        | 43.010.752 msg/s	    | 9.64 sec        |
|Akka.NET 1.1.4 (beta)	|	-                     | 8.05 sec        |
|ProtoActor C# (0.1.1)  |	61.538.461 msg/s	    | 1.29 sec        |
|Orleans 1.4.0          | -                     | -               |
|Akka 2.4.17            | -                     | -               |


## How to run
### C# examples
1. Install .NET Core SDK 1.1.1
2. Go to the benchmack folder
3. Run `dotnet restore` and `dotnet build -c Release`
4. Go to the folder '/bin/Release/net462' and run an executable file

For `ProtoActor` you also could run a NetCore version of benchmark, just type `dotnet run -c Release`

### Scala examples
1. TBD
