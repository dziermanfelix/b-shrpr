using System.Net;
using System.Net.Http.Json;
using Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Api.Tests.Integration;

public class UsersApiTests
{
    private static WebApplicationFactory<Program> CreateFactory() =>
        new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Warning);
                });
                builder.UseEnvironment("Testing");
            });

    [Fact]
    public async Task GetUsers_WhenEmpty_ReturnsEmptyArray()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task PostUsers_WithValidInput_CreatesUser()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var input = new UserInput("John Doe", "johndoe", "john@example.com");

        // Act
        var response = await client.PostAsJsonAsync("/api/users", input);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.NotEmpty(user.Id);
        Assert.Equal("John Doe", user.Name);
        Assert.Equal("johndoe", user.Username);
        Assert.Equal("john@example.com", user.Email);
        
        // Verify location header
        var location = response.Headers.Location?.ToString();
        Assert.NotNull(location);
        Assert.Contains($"/api/users/{user.Id}", location);
    }

    [Fact]
    public async Task PostUsers_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "janedoe", "john@example.com");

        // Act
        await client.PostAsJsonAsync("/api/users", input1);
        var response = await client.PostAsJsonAsync("/api/users", input2);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("email 'john@example.com' already exists", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostUsers_WithDuplicateUsername_ReturnsConflict()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "johndoe", "jane@example.com");

        // Act
        await client.PostAsJsonAsync("/api/users", input1);
        var response = await client.PostAsJsonAsync("/api/users", input2);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("username 'johndoe' already exists", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetUsers_AfterCreatingUsers_ReturnsOrderedList()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        
        await client.PostAsJsonAsync("/api/users", new UserInput("Charlie", "charlie", "charlie@example.com"));
        await client.PostAsJsonAsync("/api/users", new UserInput("Alice", "alice", "alice@example.com"));
        await client.PostAsJsonAsync("/api/users", new UserInput("Bob", "bob", "bob@example.com"));

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
        Assert.Equal(3, users.Count);
        Assert.Equal("Alice", users[0].Name);
        Assert.Equal("Bob", users[1].Name);
        Assert.Equal("Charlie", users[2].Name);
    }

    [Fact]
    public async Task PostUsers_WithCaseInsensitiveDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "janedoe", "JOHN@EXAMPLE.COM");

        // Act
        await client.PostAsJsonAsync("/api/users", input1);
        var response = await client.PostAsJsonAsync("/api/users", input2);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
