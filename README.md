# grpcdemo-c-sharp

gRPC sample implementation of server and client in C#.

This is a little sample implementation of fictional "Phone book" register that shows some principles:
* proto files
* auto generated code from proto files
* server implementation
* implementation of simple singular request/response
* implementation of stream response
* `easter egg` - implementation of bi-directional stream

## Contents

* `GrpcServer` - gRPC server implementation in C#
* `GrpcConsoleClient` - gRPC client implementation as a console application

## Prerequisites
* .net core 3.1 (or higher)
* Visual Studio 16.4 (or higher)

## How to open and run

### Preparation in Visual Studio
Simply clone the repository and open `sln` in Visual Studio.

Right click on solution and go to `Properties` - navigate to `Common properties - Startup Project`.

Make sure that `Multiple startup projects` is selected, both projects (`GrpcServer` and `GrpcConsoleClient`) are set to `Start`.

Finally make sure that `GrpcServer` is the first in order to be run.

### Run

Now you can just click on `Start` (or F5)

## What will happen on a startup

* gRPC server implementation in C#
* gRPC client implementation in C# as a Console application
* Phone book repository is a List that will on runtime auto generate 20 contact entries with random 1,2, or 3 phone numbers on each.

## Data structure

* collection of `ContactModel` objects
* on every `ContactModel` there is collection of `PhoneNumberModel` objects

## .proto files

`.proto` files are located in `Protos` folders in both solutions

## Easter egg

Press **0** on the client to see 20 seconds of banter *I'm not your buddy guy* between server and client (demo of bi-directional stream)

# References
* https://grpc.io/ - official grpc website
* https://grpc.io/docs/tutorials/basic/csharp/ - basic tutorials for C#
* https://github.com/grpc/grpc/blob/master/doc/statuscodes.md - status codes for gRPC
* https://developers.google.com/protocol-buffers/docs/proto3 - proto3 file definition
* https://www.youtube.com/watch?v=QyxCX2GYHxk - Tim Correy intro to gRPC
* https://www.name-generator.org.uk/ - Used to generate random names and addresses
* https://www.youtube.com/watch?v=tRfKdNxIOcQ - "I'm not your buddy guy" :)
