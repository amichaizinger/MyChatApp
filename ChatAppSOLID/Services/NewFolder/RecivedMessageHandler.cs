using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatAppSolid.Models;
using ChatAppSOLID.Models;
using ChatAppSOLID.Services.Commands.interfaces;
using ChatAppSOLID.Services.Interfaces;
using ChatAppSOLID.ViewModels;

namespace ChatAppSOLID.Services.NewFolder
{
    public class RecivedMessageHandler : IRecivedMessageHandler
    {

        private readonly MainViewModel _viewModel = new MainViewModel();

        public event EventHandler<string> LoginSuccess;
        public event EventHandler<string> LoginFailure;
        public event EventHandler<string> RegisterSuccess;
        public event EventHandler<string> RegisterFailure;


        public async Task RecivedCommandHandlerAsync(Socket clientSocket)
        {
            try
            {
                while (_viewModel.chatClient.IsConnected)
                {
                    byte[] buffer = new byte[1024];
                    int bytesReceived = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
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
            }
            catch (Exception ex)
            {
                _viewModel.OnErrorOccurred($"Socket error: {ex.Message}");
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
        }


        private async Task HandleLoginAsync(Message message)
        {
            var login = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(logi => logi.Trim()).Where(logi => !string.IsNullOrEmpty(logi)).ToList();

            string isCorrect = login[0];
            string username = login[1];
            Guid userId = Guid.Parse(login[2]);

            if (isCorrect == "success")
            {
                _viewModel.OnLoginSuccess(username, userId);
                LoginSuccess?.Invoke(this, username);

            }

            else
            {
                LoginFailure?.Invoke(this, "User name or password are incorrect");
            }

        }

        private async Task HandleRegisterAsync(Message message)
        {
            
                var register = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                               .Select(regi => regi.Trim()).Where(regi => !string.IsNullOrEmpty(regi)).ToList();

                string isRegistered = register[0];
                string username = register[1];
                Guid userId = Guid.Parse(register[2]);


                if (isRegistered == "success")
                {
                    _viewModel.OnLoginSuccess(username, userId);
                    RegisterSuccess?.Invoke(this, username);

                }

                else
                {
                    RegisterFailure?.Invoke(this, "User name or password are incorrect");
                }

        }


        private async Task HandleGetHistoryAsync(Message message) 
        {
            List<Chat>?chats = JsonSerializer.Deserialize<List<Chat>>(message.Content);

            _viewModel.OnChatHistoryReceived(chats);

        }

        private async Task HandleSendMessageAsync(Message message)
        {

            _viewModel.OnMessageReceived(message);
        }

        private async Task HandleCreateGroupAsync(Message message)
        {
            try
            {
                Group group = JsonSerializer.Deserialize<Group>(message.Content);

                _viewModel.OnGroupCreated(group);
            }

            catch (Exception ex)
            {
                _viewModel.OnErrorOccurred($"Failed to create group: {ex.Message}");
            }
        }

        private async Task HandleLeaveGroupAsync(Message message)
        {
            try
            {
                _viewModel.OnGroupLeft(message.GroupId.Value);
            }
            catch (Exception ex)
            {
                _viewModel.OnErrorOccurred($"Failed to leave group: {ex.Message}");
            }
        }


        private async Task HandleGetOnlineUsersAsync(Message message)
        {
            List<User>? online = JsonSerializer.Deserialize<List<User>>(message.Content);


            _viewModel.OnOnlineUsersReceived(online);
        }

        private async Task HandleAddedToGroupAsync(Message message)
        {
            Group group = JsonSerializer.Deserialize<Group>(message.Content);

            _viewModel.OnAddedToGroup(group); // todo: send who was added so the client will update, for this i sent the full message with the list
        }

        private async Task HandleGetNewUser(Message message)
        {
            User user = JsonSerializer.Deserialize<User>(message.Content);
            _viewModel.OnNewUserReceived(user);
        }


    }
}
