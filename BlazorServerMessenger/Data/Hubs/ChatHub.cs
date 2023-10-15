using Microsoft.AspNetCore.SignalR;

namespace BlazorServerMessenger.Data.Hubs;

public class ChatHub : Hub
{
    /*public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }*/

    public async Task SendChatMessage(int chatId)
    {
        await Clients.All.SendAsync("ReceiveChatMessage");
    }
}
