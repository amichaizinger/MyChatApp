using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatApp.Server.Commands;
using ChatApp.Server.Models;
using ChatApp.Server.Services.Commands;
using ChatApp.Server.Services.Commands.interfaces;
using ChatApp.Server.Services.interfaces;
using ChatApp.Server.Services.Interfaces;
using ChatApp.Server.Services.NewFolder;
using ChatAppSOLID.Services.Commands;
using ChatAppSOLID.Services.NewFolder;
using static ChatApp.Server.Models.Message;



namespace ChatApp.Server.Services
{

    public class ClientHandler : IClientHandler
    {
        private readonly IUserService _userService = new UserService();
        private readonly IMessageService _messageService = new MessageService();
        private readonly IGroupService _groupService = new GroupService();
        private Dictionary<Guid, Socket> clients = new Dictionary<Guid, Socket>();


        public async Task HandleClientAsync(Socket clientSocket)
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
                        await HandleLoginAsync(message, clientSocket);
                    }
                    else if (message.Command == CommandType.Register)
                    {
                        await HandleRegisterAsync(message, clientSocket);
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
                        await HandleLeaveGroupAsync(message, clientSocket);
                    }
                    else if (message.Command == CommandType.GetOnlineUsers)
                    {
                        await HandleGetOnlineUsersAsync(clientSocket);
                    }
                    else if (message.Command == CommandType.AddUserToGroup)
                    {
                        await HandleAddUserToGroupAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed: {ex.Message}");
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
        }

        private async Task HandleLoginAsync(Message message, Socket clientSocket)
        {
            var login = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(logi => logi.Trim()).Where(logi => !string.IsNullOrEmpty(logi)).ToList();

            string username = login[0];
            string password = login[1];

            User user = await _userService.LoginAsync(username, password);

            if (user != null)
            {
                ICommand loginCommand = new LoginCommand(username, user.Id, "success");
                await loginCommand.ExecuteAsync(clientSocket);
                clients[user.Id] = clientSocket; // saving the socket with the username
                HandleGetChatHistoryAsync(user.Id, clientSocket);
                HandleGetOnlineUsersAsync(clientSocket);

            }
            else
            {
                ICommand loginCommand = new LoginCommand(username, user.Id, "failure");
                await loginCommand.ExecuteAsync(clientSocket);
            }

        }

        private async Task HandleRegisterAsync(Message message, Socket clientSocket)
        {
            try
            {
                Console.WriteLine(message.Content);
                var register = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                               .Select(regi => regi.Trim()).Where(regi => !string.IsNullOrEmpty(regi)).ToList();

                string username = register[0];
                string password = register[1];

                User user = await _userService.RegisterAsync(username, password);

                if (user != null)
                {
                    ICommand registerCommand = new RegisterCommand(username, user.Id, "success");
                    await registerCommand.ExecuteAsync(clientSocket);
                    clients[user.Id] = clientSocket;

                    foreach(var client in clients)
                    {
                        if (client.Key != user.Id)
                        {
                            ICommand newUser = new NewUserCommand(user);
                            await newUser.ExecuteAsync(client.Value);
                        }
                    }
                }

                else
                {
                    ICommand registerCommand = new RegisterCommand(username, user.Id, "failure");
                    await registerCommand.ExecuteAsync(clientSocket);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Registration failed: {ex.Message}");
            }
        }

        private async Task HandleGetChatHistoryAsync(Guid userId, Socket clientSocket)
        {
            var chatHistory = await _messageService.GetChatHistoryAsync(userId);
            ICommand getHistory = new GetChatHistoryCommand(userId, chatHistory);
            await getHistory.ExecuteAsync(clientSocket);
        }

        private async Task HandleSendMessageAsync(Message message)
        {
            message.Status = MessageStatus.Sent;
            await _messageService.SaveMessageAsync(message);

            ICommand sendMessage = new SendMessageCommand(message);


            if (message.GroupId.HasValue)
            {
                var members = await _groupService.GetGroupMembersAsync(message.GroupId.Value);
                foreach (var member in members)
                {
                    if (clients.TryGetValue(member.Id, out Socket memberSocket))
                    {
                        await sendMessage.ExecuteAsync(memberSocket);
                        message.Status = MessageStatus.Recived;
                    }
                }
            }
            else if (message.ReciverId.HasValue)
            {
                if (clients.TryGetValue(message.ReciverId.Value, out Socket receiverSocket))
                {
                    await sendMessage.ExecuteAsync(receiverSocket);
                    message.Status = MessageStatus.Recived;
                }
                // todo: send who got the message, and also when someone reconnects inform
            }
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

                await _groupService.CreateGroupAsync(GroupName, members);

                ICommand createGroup = new CreateGroupCommand(message);

                foreach (var member in members)
                {
                    if (clients.TryGetValue(member, out Socket memberSocket))
                    {
                        await createGroup.ExecuteAsync(memberSocket);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create group: {ex.Message}");
            }
        }

        private async Task HandleLeaveGroupAsync(Message message, Socket clientSocket)
        {
            try
            {
                await _groupService.RemoveUserFromGroupAsync(message.GroupId.Value, message.SenderId);

                ICommand leaveGroup = new LeaveGroupCommand(message.GroupId.Value, message.SenderId);
                await leaveGroup.ExecuteAsync(clientSocket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to leave group: {ex.Message}");
            }
        }


        private async Task HandleGetOnlineUsersAsync(Socket clientSocket)
        {
            var onlineUsers = clients.Keys.ToList();
            ICommand getOnline = new GetOnlineUsersCommand(onlineUsers);
            await getOnline.ExecuteAsync(clientSocket);
        }

        private async Task HandleAddUserToGroupAsync(Message message)
        {
            var userIds = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToList();

            foreach (var userId in userIds)
            {
                await _groupService.AddUserToGroupAsync(message.GroupId.Value, Guid.Parse(userId));
                ICommand addUsers = new AddUserToGroupCommand(message);

                foreach (var member in userIds)
                {
                    if (clients.TryGetValue(Guid.Parse(member), out Socket memberSocket))
                    {
                        await addUsers.ExecuteAsync(memberSocket);
                    }
                }  // todo: send who was added so the client will update

            }
        }

    }
}