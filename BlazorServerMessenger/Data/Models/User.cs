using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerMessenger.Data.Models;

public class User : IdentityUser<int>
{
    public string PublicInfo { get; set; } = string.Empty;
}
