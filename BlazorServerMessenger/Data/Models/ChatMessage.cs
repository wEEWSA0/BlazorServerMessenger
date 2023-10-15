using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerMessenger.Data.Models;

[PrimaryKey(nameof(ChatId), nameof(Id))]
public class ChatMessage
{
    public int ChatId { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public DateTime Timestamp { get; set; } // TODO переименовать
}
