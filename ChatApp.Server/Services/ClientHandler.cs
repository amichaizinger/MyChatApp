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
        private Dictionary<string, Socket> clients = new Dictionary<string, Socket>();


        public async Task HandleClientAsync(Socket clientSocket)
        {
            try
            {
                while (clientSocket.Connected)
                {
                    byte[] buffer = new byte[8192];
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

            var user = await _userService.LoginAsync(username, password);

            if (user != null)
            {
                ICommand loginCommand = new LoginCommand(username, user.Id, "success");
                await loginCommand.ExecuteAsync(clientSocket);
                clients[user.Id] = clientSocket; // saving the socket with the username
                await HandleGetChatHistoryAsync(user.Id, clientSocket);
                await HandleGetOnlineUsersAsync(clientSocket);

            }
            else
            {
                ICommand loginCommand = new LoginCommand("youLoser", "lalalalalala", "failure");
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
                    await HandleGetChatHistoryAsync(user.Id, clientSocket);
                    await HandleGetOnlineUsersAsync(clientSocket);

                    foreach (var client in clients)
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
                    ICommand registerCommand = new RegisterCommand("whatsappsocks", "useChatAppInstead", "failure");
                    await registerCommand.ExecuteAsync(clientSocket);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Registration failed: {ex.Message}");
            }
        }

        private async Task HandleGetChatHistoryAsync(string userId, Socket clientSocket)
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


            if (!string.IsNullOrEmpty(message.GroupId))
            {
                var members = await _groupService.GetGroupMembersAsync(message.GroupId);
                foreach (var member in members)
                {
                    if (clients.TryGetValue(member.Id, out Socket memberSocket))
                    {
                        await sendMessage.ExecuteAsync(memberSocket);
                        message.Status = MessageStatus.Recived;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(message.ReciverId))
            {
                if (clients.TryGetValue(message.ReciverId, out Socket receiverSocket))
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
                var group = JsonSerializer.Deserialize<Group>(message.Content);


                await _groupService.NewGroupAsync(group);

                ICommand createGroup = new CreateGroupCommand(message);

                foreach (var member in group.Members)
                {
                    if (clients.TryGetValue(member.Id, out Socket memberSocket))
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
                await _groupService.RemoveUserFromGroupAsync(message.GroupId, message.SenderId);

                ICommand leaveGroup = new LeaveGroupCommand(message.GroupId, message.SenderId);
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
            var userIds = JsonSerializer.Deserialize<List<string>>(message.Content);

            foreach (var userId in userIds)
            {
                await _groupService.AddUserToGroupAsync(message.GroupId, userId);
                ICommand addUsers = new AddUserToGroupCommand(message);

                foreach (var member in userIds)
                {
                    if (clients.TryGetValue(member, out Socket memberSocket))
                    {
                        await addUsers.ExecuteAsync(memberSocket);
                    }
                }  // todo: send who was added so the client will update

            }
        }

    }
}