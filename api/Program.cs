using Api.Models;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<UserStore>();

var app = builder.Build();

app.UseCors();

app.MapGet("/api/users", (UserStore store) => Results.Ok(store.GetUsers()));
app.MapPost("/api/users", (UserInput input, UserStore store) =>
{
    try
    {
        var user = store.CreateUser(input);
        return Results.Created($"/api/users/{user.Id}", user);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.Run();
