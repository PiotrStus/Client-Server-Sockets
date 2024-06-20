# Server-Client application

This project contains a simple server and client implementation written in C# using TCP/IP sockets. 
The server allows the client to send simple commands, to which it responds according to the data implemented in the code. The server and client exchange information using JSON.

## Video

[![Alt text for your video](http://img.youtube.com/vi/Al10hUIZXU8/0.jpg)](http://www.youtube.com/watch?v=Al10hUIZXU8)

## How to Run

1. Clone this project to your local environment.
2. Open the project in any C# compatible IDE.
3. Run the server application and then run the client application.
4. The client should establish a connection with the server, and the client will be able to send commands to the server.

## Features

Currently, the project has the following features:
- Command handling:
1. `help` - list of available commands,
2. `info` - server version & creation date,
3.  `uptime` - server's lifetime,
4.  `stop` - stops server and the client,
5.  `login` - login user,
6.  `register` - register user
- Additional commands for a normal user:
1.  `logout` - logout user,
2.  `message` - send a message,
3.  `mailbox` - check your mailbox
- Additional commands for the admin user:
1. `delete` - delete a user.
- Sending simple server responses to client requests.
- Basic error and exception handling.
- Data exchange between client and server using JSON.

## Potential Extensions

Some potential functionalities to add to this project include:
- Handling multiple clients concurrently through multi-threading or asynchronous I/O.
- Expanding the list of available commands with new features such as file transfer, user management, etc.
- Adding event and error logging to a file for better debugging and application monitoring.
- Implementing a graphical user interface (GUI) for the client and/or server to facilitate user interaction with the application.
