# ChatApp

A WPF-based chat application built using C# with SOLID principles, supporting private messaging and group chats on a local network.

## Features

- User registration and login
- Private messaging between users
- Group chat creation and management
- Real-time message delivery
- Online status indicators
- Message history persistence
- Modern UI with XAML styling

## Architecture

This application follows a client-server architecture with:

- **Client**: WPF application with MVVM pattern
- **Server**: Console application handling client connections and database operations
- **Database**: SQLite for user accounts, messages, and group information

## Prerequisites

- .NET Framework 4.7.2 or higher
- Visual Studio 2019 or newer
- SQLite

## Project Structure

- **ChatAppSOLID**: Client application
  - Models: Data models for users, messages, and groups
  - ViewModels: MVVM implementation
  - Views: XAML UI components
  - Services: Network communication and business logic

- **ChatApp.Server**: Server application
  - Data: Database context and operations
  - Services: Client handling and message routing
  - Commands: Command pattern implementation for different actions

## Deployment Guide

### Step 1: Clone Repository

```
git clone [repository-url]
cd ChatApp
```

### Step 2: Build Solution

Open the solution in Visual Studio and build it:

```
dotnet build
```

### Step 3: Set Up the Server

1. Navigate to the server project directory
2. Start the server application:

```
cd ChatApp.Server\bin\Debug\net6.0
.\ChatApp.Server.exe
```

The server will start listening for client connections on the default port.

### Step 4: Start Client Application(s)

1. Navigate to the client project directory
2. Start the client application:

```
cd ChatAppSOLID\bin\Debug\net6.0-windows
.\ChatAppSOLID.exe
```

3. You can start multiple client instances to test communication between different users.

### Step 5: Register and Login

- On first use, register a new account with username and password (both must be at least 8 characters)
- For subsequent uses, login with your credentials

## Configuration

The application is currently configured for local network use. You can modify the connection settings:

### Client Configuration
Edit the `ClientSocket.cs` file to change the server address if deploying outside the local network:

```csharp
// Default is "127.0.0.1" for local connections
_socket.Connect(new IPEndPoint(IPAddress.Parse("your_server_ip"), 5000));
```

### Server Configuration
Edit the `Program.cs` in the server project to change the listening address and port:

```csharp
// Default listens on all interfaces at port 5000
server.Start(IPAddress.Any, 5000);
```

## Extending for Internet Use

To make this application work over the internet:

1. Ensure the server has a public IP or is properly port-forwarded
2. Update the client to connect to the public IP of the server
3. Consider implementing SSL/TLS for secure communication
4. Add authentication tokens for session management

## Troubleshooting

- **Connection Issues**: Ensure server is running and firewall is not blocking the connection
- **Database Errors**: Check SQLite file permissions
- **UI Not Updating**: Review the PropertyChanged notifications in ViewModels

- 
## Acknowledgements

- Created as a demonstration of SOLID principles in a real-world application
- Inspired by modern chat applications like WhatsApp and Telegram