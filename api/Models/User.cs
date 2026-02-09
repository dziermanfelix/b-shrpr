namespace Api.Models;

public record User
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record UserInput(string Name, string Username, string Email);
