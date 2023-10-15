using System.ComponentModel.DataAnnotations;

namespace BlazorServerMessenger.Data.Models;

public class Chat
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public List<ChatUser> ChatUsers { get; set; } = new();
    public List<ChatMessage> Messages { get; set; } = new();
    // TODO add IsPersonalChat
}
