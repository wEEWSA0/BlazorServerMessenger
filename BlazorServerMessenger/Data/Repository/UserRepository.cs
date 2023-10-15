using BlazorServerMessenger.Data.Models;

using Microsoft.EntityFrameworkCore;

using System.Xml.Linq;

namespace BlazorServerMessenger.Data.Repository;

public class UserRepository
{
    private ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public (string, string) GetUserNameAndPublicInfo(int id)
    {
        var user = _dbContext.Users.AsNoTracking().Where(u => u.Id == id).SingleOrDefault();

        return (user!.UserName!, user.PublicInfo);
    }

    public User GetUserByNameOrThrow(string name)
    {
        var user = _dbContext.Users.AsNoTracking().SingleOrDefault(u => u.UserName == name);

        if (user == null)
            throw new ArgumentException("Пользователь не найден");

        return user;
    }

    public List<User> GetUsersContainsName(string name)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return _dbContext.Users.Where(u => u.NormalizedUserName!.Contains(name.ToUpper()))
            .AsNoTracking()
            .ToList();
    }

    public void UpdateUserPublicInfo(int id, string publicInfo)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Id == id)!;

        user.PublicInfo = publicInfo;

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }
}
