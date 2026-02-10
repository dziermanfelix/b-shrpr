using Api.Models;
using Api.Services;
using Xunit;

namespace Api.Tests.Services;

public class UserStoreTests
{
    [Fact]
    public void GetUsers_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        var store = new UserStore();

        // Act
        var result = store.GetUsers();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetUsers_ReturnsUsersOrderedByName()
    {
        // Arrange
        var store = new UserStore();
        var user1 = store.CreateUser(new UserInput("Charlie", "charlie", "charlie@example.com"));
        var user2 = store.CreateUser(new UserInput("Alice", "alice", "alice@example.com"));
        var user3 = store.CreateUser(new UserInput("Bob", "bob", "bob@example.com"));

        // Act
        var result = store.GetUsers();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal("Charlie", result[2].Name);
    }

    [Fact]
    public void GetUsers_IsCaseInsensitive()
    {
        // Arrange
        var store = new UserStore();
        var user1 = store.CreateUser(new UserInput("charlie", "charlie", "charlie@example.com"));
        var user2 = store.CreateUser(new UserInput("Alice", "alice", "alice@example.com"));
        var user3 = store.CreateUser(new UserInput("BOB", "bob", "bob@example.com"));

        // Act
        var result = store.GetUsers();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("BOB", result[1].Name);
        Assert.Equal("charlie", result[2].Name);
    }

    [Fact]
    public void CreateUser_WithValidInput_CreatesUser()
    {
        // Arrange
        var store = new UserStore();
        var input = new UserInput("John Doe", "johndoe", "john@example.com");

        // Act
        var result = store.CreateUser(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("johndoe", result.Username);
        Assert.Equal("john@example.com", result.Email);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);
        Assert.True(result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void CreateUser_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var store = new UserStore();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "janedoe", "john@example.com");
        store.CreateUser(input1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => store.CreateUser(input2));
        Assert.Contains("email 'john@example.com' already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateUser_WithDuplicateEmailCaseInsensitive_ThrowsInvalidOperationException()
    {
        // Arrange
        var store = new UserStore();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "janedoe", "JOHN@EXAMPLE.COM");
        store.CreateUser(input1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => store.CreateUser(input2));
        Assert.Contains("email 'JOHN@EXAMPLE.COM' already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateUser_WithDuplicateUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var store = new UserStore();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "johndoe", "jane@example.com");
        store.CreateUser(input1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => store.CreateUser(input2));
        Assert.Contains("username 'johndoe' already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateUser_WithDuplicateUsernameCaseInsensitive_ThrowsInvalidOperationException()
    {
        // Arrange
        var store = new UserStore();
        var input1 = new UserInput("John Doe", "johndoe", "john@example.com");
        var input2 = new UserInput("Jane Doe", "JOHNDOE", "jane@example.com");
        store.CreateUser(input1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => store.CreateUser(input2));
        Assert.Contains("username 'JOHNDOE' already exists", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateUser_WhenUsernameConflict_RollbackEmailAllowsReuse()
    {
        // When duplicate username fails, the tentatively reserved email is rolled back
        // so the same email can be used in a subsequent successful create.
        var store = new UserStore();
        store.CreateUser(new UserInput("John Doe", "johndoe", "john@example.com"));

        // Fail: duplicate username, new email (jane@example.com was tentatively added then rolled back)
        Assert.Throws<InvalidOperationException>(() =>
            store.CreateUser(new UserInput("Jane Doe", "johndoe", "jane@example.com")));

        // Succeed: jane@example.com is still available after rollback
        var user = store.CreateUser(new UserInput("Jane Doe", "janedoe", "jane@example.com"));
        Assert.Equal("jane@example.com", user.Email);
        Assert.Equal(2, store.GetUsers().Count);
    }

    [Fact]
    public void CreateUser_GeneratesUniqueIds()
    {
        // Arrange
        var store = new UserStore();
        var input1 = new UserInput("User One", "user1", "user1@example.com");
        var input2 = new UserInput("User Two", "user2", "user2@example.com");

        // Act
        var user1 = store.CreateUser(input1);
        var user2 = store.CreateUser(input2);

        // Assert
        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void CreateUser_StoresUserInStore()
    {
        // Arrange
        var store = new UserStore();
        var input = new UserInput("Test User", "testuser", "test@example.com");

        // Act
        var createdUser = store.CreateUser(input);
        var users = store.GetUsers();

        // Assert
        Assert.Single(users);
        Assert.Equal(createdUser.Id, users[0].Id);
        Assert.Equal("Test User", users[0].Name);
    }
}
