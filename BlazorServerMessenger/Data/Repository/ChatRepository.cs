using BlazorServerMessenger.Data.Models;

using Microsoft.EntityFrameworkCore;

using System;

namespace BlazorServerMessenger.Data.Repository;

public class ChatRepository
{
    private ApplicationDbContext _dbContext;

    public ChatRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public bool TryGetChatBetweenUsers(int ourUserId, int userId, out Chat chat)
    {
        chat = _dbContext.Chats
            .Include(c => c.ChatUsers)
            .ThenInclude(cu => cu.User)
            .Include(c => c.Messages)
            .FirstOrDefault(c => c.ChatUsers.Any(cu => cu.UserId == ourUserId) && c.ChatUsers.Any(cu => cu.UserId == userId));

        return chat is not null;
    }

    public Chat CreateOrGetChatBetweenUsers(int ourUserId, int userId, string chatName)
    {
        var ourUser = _dbContext.Users.Find(ourUserId);
        var otherUser = _dbContext.Users.Find(userId);

        if (ourUser == null || otherUser == null)
            throw new ArgumentException("Пользователь не найден");

        var existingChat = _dbContext.Chats
            .Include(c => c.ChatUsers)
            .ThenInclude(cu => cu.User)
            .FirstOrDefault(c => c.ChatUsers.Any(cu => cu.UserId == ourUserId) && c.ChatUsers.Any(cu => cu.UserId == userId));

        if (existingChat != null)
            return existingChat; // Чат уже существует, возвращаем его

        var newChat = new Chat { Name = chatName };
        newChat.ChatUsers.Add(new ChatUser { UserId = ourUserId });
        newChat.ChatUsers.Add(new ChatUser { UserId = userId });

        _dbContext.Chats.Add(newChat);
        _dbContext.SaveChanges();

        return newChat;
    }

    [Obsolete]
    public List<ChatMessage> GetAllChatMessages(int chatId)
    {
        var chat = _dbContext.Chats
            .Include(c => c.Messages)
            .ThenInclude(m => m.Sender)
            .FirstOrDefault(c => c.Id == chatId);

        if (chat == null)
            throw new ArgumentException("Чат не найден");

        chat.Messages.Reverse();

        return chat.Messages;
    }

    public List<ChatMessage> GetLastChatMessages(int chatId, int count)
    {
        if (count < 1)
        {
            throw new ArgumentException($"{count} less than 1");
        }

        var lastMessages = _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.Timestamp)
            .Take(count)
            //.OrderBy(m => m.Timestamp)
            .Include(c => c.Sender)
            .ToList();

        lastMessages.Reverse();

        return lastMessages;
    }

    public List<ChatMessage> GetLastChatMessagesInRange(int chatId, int startIndex, int count)
    {
        if (count < 1)
        {
            throw new ArgumentException($"{count} less than 1");
        }

        var lastMessages = _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.Timestamp)
            .Skip(startIndex)
            .Take(count)
            .Include(c => c.Sender)
            .ToList();

        return lastMessages;
    }

    public void SendMessage(int chatId, int senderId, string messageContent)
    {
        var chat = _dbContext.Chats
            .Include(c => c.Messages)
            .Include(c => c.ChatUsers)
            .ThenInclude(u => u.User)
            .FirstOrDefault(c => c.Id == chatId);

        if (chat == null)
            throw new ArgumentException("Чат не найден");

        if (!chat.ChatUsers.Any(u => u.UserId == senderId))
            throw new ArgumentException("Пользователь не найден");

        var newMessage = new ChatMessage
        {
            ChatId = chatId,
            Content = messageContent,
            SenderId = senderId,
            Timestamp = DateTime.UtcNow
        };

        _dbContext.Messages.Add(newMessage);
        _dbContext.SaveChanges();
    }

    public List<Chat> GetUserChats(int userId)
    {
        return _dbContext.ChatUsers
            .Where(cu => cu.UserId == userId)
            .Include(cu => cu.Chat)
            .Select(cu => cu.Chat)
            .ToList();
    }

    public List<Chat> GetUserChatsWithOnlyLastMessage(int userId)
    {
        var chats = _dbContext.ChatUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == userId)
            .Include(cu => cu.Chat)
            .Select(cu => cu.Chat)
            .ToList();

        foreach (var chat in chats)
        {
            var lastMessage = _dbContext.Messages
                .Where(m => m.ChatId == chat.Id)
                .OrderByDescending(p => p.Timestamp)
                .FirstOrDefault(); // TODO Проверить работу

            if (lastMessage is null)
            {
                chat.Messages = null!;

                continue;
            }

            chat.Messages = new() { lastMessage! };
        }

        return chats;
    }
}
