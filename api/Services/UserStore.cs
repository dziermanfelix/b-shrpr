using System.Collections.Concurrent;
using Api.Models;

namespace Api.Services;

public class UserStore
{
    private readonly ConcurrentDictionary<string, User> _users = new();
    private readonly ConcurrentDictionary<string, string> _emailsByNormalized = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _usernamesByNormalized = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<User> GetUsers() => _users.Values.OrderBy(u => u.Name, StringComparer.OrdinalIgnoreCase).ToList();

    public User CreateUser(UserInput input)
    {
        if (_emailsByNormalized.TryAdd(input.Email, input.Email) == false)
            throw new InvalidOperationException($"A user with email '{input.Email}' already exists.");
        if (_usernamesByNormalized.TryAdd(input.Username, input.Username) == false)
        {
            _emailsByNormalized.TryRemove(input.Email, out _);
            throw new InvalidOperationException($"A user with username '{input.Username}' already exists.");
        }

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
