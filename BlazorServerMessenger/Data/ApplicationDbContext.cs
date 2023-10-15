using BlazorServerMessenger.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerMessenger.Data;
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> Messages { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }
}
