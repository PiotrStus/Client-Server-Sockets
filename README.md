# Server-Client application

This project contains a simple server and client implementation written in C# using TCP/IP sockets. The server allows the client to send simple commands, to which it responds according to the data implemented in the code. This is a sample project that can be further developed and expanded with new functionalities.

## Video

[![Alt text for your video](http://img.youtube.com/vi/-J5Yr98vHas/0.jpg)](http://www.youtube.com/watch?v=-J5Yr98vHas)


## How to Run

1. Clone this project to your local environment.
2. Open the project in any C# compatible IDE.
3. Run the server application and then run the client application.
4. The client should establish a connection with the server, and the client will be able to send commands to the server.

## Features

Currently, the project has the following features:
- Command handling: `help`, `info`, `uptime`, `stop`.
- Sending simple server responses to client queries.
- Basic error and exception handling.

## Potential Extensions

Some potential functionalities to add to this project include:
- Handling multiple clients concurrently through multi-threading or asynchronous I/O.
- Expanding the list of available commands with new features such as file transfer, user management, etc.
- Adding event and error logging to a file for better debugging and application monitoring.
- Implementing a graphical user interface (GUI) for the client and/or server to facilitate user interaction with the application.
