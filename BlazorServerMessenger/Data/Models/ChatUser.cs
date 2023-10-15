﻿using Microsoft.EntityFrameworkCore;

namespace BlazorServerMessenger.Data.Models;

[PrimaryKey(nameof(ChatId), nameof(UserId))]
public class ChatUser
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}
