using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ChatAppSolid.Models;
using ChatAppSolid.Models.ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Commands.interfaces;
using ChatAppSOLID.Services.Interfaces;
using ChatAppSOLID.ViewModels;

namespace ChatAppSOLID.Services.NewFolder
{
    public class RecivedMessageHandler : IRecivedMessageHandler
    {

        public MainViewModel mainViewModel = new MainViewModel();

        public event EventHandler<string> LoginSuccess;
        public event EventHandler<string> LoginFailure;
        public event EventHandler<string> RegisterSuccess;
        public event EventHandler<string> RegisterFailure;


        public async Task RecivedCommandHandlerAsync(Socket clientSocket)
        {
            try
            {
                while (mainViewModel.chatClient.IsConnected)
                {
                    // Ensure socket is valid
                    if (clientSocket == null || !clientSocket.Connected)
                    {
                        mainViewModel.chatClient.IsConnected = false;
                        mainViewModel.OnErrorOccurred("Socket is not connected.");
                        break;
                    }

                    try
                    {
                        string json = await ReadMessageAsync(clientSocket);
                        Message message = JsonSerializer.Deserialize<Message>(json);

                        if (message.Command == CommandType.Login)
                        {
                            await HandleLoginAsync(message);
                        }
                        else if (message.Command == CommandType.Register)
                        {
                            await HandleRegisterAsync(message);
                        }
                        else if (message.Command == CommandType.SendMessage)
                        {
                            await HandleSendMessageAsync(message);
                        }
                        else if (message.Command == CommandType.CreateGroup)
                        {
                            await HandleCreateGroupAsync(message);
                        }
                        else if (message.Command == CommandType.LeaveGroup)
                        {
                            await HandleLeaveGroupAsync(message);
                        }
                        else if (message.Command == CommandType.GetOnlineUsers)
                        {
                            await HandleGetOnlineUsersAsync(message);
                        }
                        else if (message.Command == CommandType.AddUserToGroup)
                        {
                            await HandleAddedToGroupAsync(message);
                        }
                        else if (message.Command == CommandType.GetChatHistory)
                        {
                            await HandleGetHistoryAsync(message);
                        }
                        else if (message.Command == CommandType.GetNewUser)
                        {
                            await HandleGetNewUser(message);
                        }
                    }
                    catch (JsonException ex)
                    {
                        mainViewModel.OnErrorOccurred($"Failed to deserialize message: {ex.Message}");
                        Debug.WriteLine($"Failed to deserialize message: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        mainViewModel.OnErrorOccurred($"Error processing message: {ex.Message}");
                        Debug.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }
            catch (SocketException ex)
            {
                mainViewModel.chatClient.IsConnected = false;
                mainViewModel.OnErrorOccurred($"Socket error: {ex.Message}");
                Debug.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                mainViewModel.chatClient.IsConnected = false;
                mainViewModel.OnErrorOccurred($"Unexpected error in receive loop: {ex.Message}");
                Debug.WriteLine($"Unexpected error in receive loop: {ex.Message}");
            }
            finally
            {
                if (mainViewModel.chatClient.IsConnected)
                {
                    try
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        clientSocket.Dispose();
                    }
                    catch (Exception ex)
                    {
                        mainViewModel.OnErrorOccurred($"Failed to shutdown socket: {ex.Message}");
                        Debug.WriteLine($"Failed to shutdown socket: {ex.Message}");
                    }
                }
                mainViewModel.chatClient.IsConnected = false;
            }
        }

        private async Task<string> ReadMessageAsync(Socket socket)
        {
            // Read the 4-byte length prefix
            byte[] lengthBytes = new byte[4];
            int bytesRead = 0;
            while (bytesRead < 4)
            {
                int received = await socket.ReceiveAsync(lengthBytes.AsMemory(bytesRead, 4 - bytesRead), SocketFlags.None);
                if (received == 0)
                {
                    throw new Exception("Socket closed by server");
                }
                bytesRead += received;
            }
            int length = BitConverter.ToInt32(lengthBytes, 0);

            // Read the message bytes
            byte[] messageBytes = new byte[length];
            bytesRead = 0;
            while (bytesRead < length)
            {
                int received = await socket.ReceiveAsync(messageBytes.AsMemory(bytesRead, length - bytesRead), SocketFlags.None);
                if (received == 0)
                {
                    throw new Exception("Socket closed by server");
                }
                bytesRead += received;
            }

            return Encoding.UTF8.GetString(messageBytes);
        }


        private async Task HandleLoginAsync(Message message)
        {
            var login = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(logi => logi.Trim()).Where(logi => !string.IsNullOrEmpty(logi)).ToList();

            string isCorrect = login[0];
            string username = login[1];
            string userId = message.SenderId;

            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        if (isCorrect == "success")
                        {
                            LoginSuccess?.Invoke(this, username);
                            mainViewModel.OnLoginSuccess(username, userId);
                        }
                        else
                        {
                            LoginFailure?.Invoke(this, "User name or password are incorrect");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Dispatcher error in HandleLoginAsync: {ex.Message}");
                        MessageBox.Show($"UI update failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InvokeAsync failed: {ex.Message}");
            }
        }

        

        private async Task HandleRegisterAsync(Message message)
        {

            var register = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Select(regi => regi.Trim()).Where(regi => !string.IsNullOrEmpty(regi)).ToList();

            string isRegistered = register[0];
            string username = register[1];
            string userId = message.SenderId;

            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        if (isRegistered == "success")
                        {
                            RegisterSuccess?.Invoke(this, username);
                            mainViewModel.OnLoginSuccess(username, userId);

                        }
                        else
                        {
                            RegisterFailure?.Invoke(this, "User name or password are incorrect");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Dispatcher error in HandleLoginAsync: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InvokeAsync failed: {ex.Message}");
            }
        }


        private async Task HandleGetHistoryAsync(Message message)
        {
            List<Chat>? chats = JsonSerializer.Deserialize<List<Chat>>(message.Content);

            mainViewModel.OnChatHistoryReceived(chats);

        }

        private async Task HandleSendMessageAsync(Message message)
        {

            mainViewModel.OnMessageReceived(message);
        }

        private async Task HandleCreateGroupAsync(Message message)
        {
            try
            {
                Group group = JsonSerializer.Deserialize<Group>(message.Content);

                mainViewModel.OnGroupCreated(group);
            }

            catch (Exception ex)
            {
                mainViewModel.OnErrorOccurred($"Failed to create group: {ex.Message}");
            }
        }

        private async Task HandleLeaveGroupAsync(Message message)
        {
            try
            {
                mainViewModel.OnGroupLeft(message.GroupId);
            }
            catch (Exception ex)
            {
                mainViewModel.OnErrorOccurred($"Failed to leave group: {ex.Message}");
            }
        }


        private async Task HandleGetOnlineUsersAsync(Message message)
        {
            List<string>? onlineId = JsonSerializer.Deserialize<List<string>>(message.Content);


            mainViewModel.OnOnlineUsersReceived(onlineId);
        }

        private async Task HandleAddedToGroupAsync(Message message)
        {
            var usersId = JsonSerializer.Deserialize<List<string>>(message.Content);

            mainViewModel.OnAddedToGroup(usersId, message.GroupId); // todo: send who was added so the client will update, for this i sent the full message with the list
        }

        private async Task HandleGetNewUser(Message message)
        {
            User user = JsonSerializer.Deserialize<User>(message.Content);
            mainViewModel.OnNewUserReceived(user);
        }


    }
}
