using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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


        public async Task RecivedCommandHandlerAsync(Socket clientSocket)
        {
            try
            {
                while (clientSocket.Connected)
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
                }
            }
            catch (Exception ex)
            {
                _viewModel.OnError($"Socket error: {ex.Message}");
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
                LoginFailure?.Invoke(this);
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
                    RegisterFailure?.Invoke(this);
                }

        }


        private async Task HandleGetHistoryAsync(Message message)
        {
            List<Message> chats = new List<Message>();

            var jsonChat = message.Content.Split('|', StringSplitOptions.RemoveEmptyEntries)
                             .Select(regi => regi.Trim()).Where(regi => !string.IsNullOrEmpty(regi)).ToList();

            foreach (var jsonMessage in jsonMessages)
            {
                Message ?historyMessge = JsonSerializer.Deserialize<Message>(jsonMessage);
                history.Add(historyMessge);
            }

            _viewModel.OnHistoryReceived(history);
        }

        private async Task HandleSendMessageAsync(Message message)
        {

            _viewModel.OnMessageReceived(message);
        }

        private async Task HandleCreateGroupAsync(Message message)
        {
            try
            {
                var group = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g)).ToList();

                string GroupName = group[0];
                List<Guid> members = group.Skip(1).Select(g => Guid.Parse(g)).ToList();
                members.Add(message.SenderId);

                Group newGroup = new Group
                {
                    Name = GroupName,
                    Members = members
                };

                _viewModel.OnGroupCreated(groupName, members);
            }

            catch (Exception ex)
            {
                _viewModel.OnError($"Failed to create group: {ex.Message}");
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
                _viewModel.OnError($"Failed to leave group: {ex.Message}");
            }
        }


        private async Task HandleGetOnlineUsersAsync(Message message)
        {
            var online = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g)).ToList();

            _viewModel.OnOnlineUsersReceived(online);
        }

        private async Task HandleAddedToGroupAsync(Message message)
        {
            _viewModel.OnAddedToGroup(message.GroupId); // todo: send who was added so the client will update, for this i sent the full message with the list
        }


    }
}
