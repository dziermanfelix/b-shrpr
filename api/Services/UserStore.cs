using System.Collections.Concurrent;
using Api.Models;

namespace Api.Services;

public class UserStore
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public IReadOnlyList<User> GetUsers() => _users.Values.OrderBy(u => u.Name, StringComparer.OrdinalIgnoreCase).ToList();

    public User CreateUser(UserInput input)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = input.Name,
            Username = input.Username,
            Email = input.Email,
            CreatedAt = DateTime.UtcNow
        };
        _users[user.Id] = user;
        return user;
    }
}
