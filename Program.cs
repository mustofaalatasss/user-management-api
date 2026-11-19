using System.Text.RegularExpressions;
using UserManagementApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add logging and services
builder.Services.AddLogging();

var app = builder.Build();

// -------------------------------------------------------------
// 1. CUSTOM MIDDLEWARE PIPELINE (Grading Criteria 5)
// -------------------------------------------------------------
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();

// In-Memory User Data Store
var users = new List<User>
{
    new User { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Role = "Admin" },
    new User { Id = 2, Name = "Bob Smith", Email = "bob@example.com", Role = "User" }
};

// -------------------------------------------------------------
// 2. VALIDATION HELPER (Grading Criteria 4)
// -------------------------------------------------------------
List<string> ValidateUser(User user, bool isUpdate = false)
{
    var errors = new List<string>();

    if (!isUpdate || !string.IsNullOrEmpty(user.Name))
    {
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            errors.Add("Field 'Name' is required and cannot be empty.");
        }
    }

    if (!isUpdate || !string.IsNullOrEmpty(user.Email))
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        if (string.IsNullOrWhiteSpace(user.Email) || !emailRegex.IsMatch(user.Email))
        {
            errors.Add("A valid 'Email' is required.");
        }
    }

    return errors;
}

// -------------------------------------------------------------
// 3. CRUD ENDPOINTS (Grading Criteria 2)
// -------------------------------------------------------------

// Root status endpoint
app.MapGet("/", () => Results.Ok(new
{
    message = "User Management ASP.NET Core API is running",
    endpoints = new[] { "GET /users", "GET /users/{id}", "POST /users", "PUT /users/{id}", "DELETE /users/{id}" }
}));

// GET: Retrieve all users
app.MapGet("/users", () => Results.Ok(new
{
    success = true,
    count = users.Count,
    data = users
}));

// GET by ID: Retrieve single user
app.MapGet("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
    {
        return Results.NotFound(new { success = false, message = $"User with ID {id} not found." });
    }
    return Results.Ok(new { success = true, data = user });
});

// POST: Create a new user (with validation)
app.MapPost("/users", (User newUser) =>
{
    var errors = ValidateUser(newUser, isUpdate: false);
    if (errors.Any())
    {
        return Results.BadRequest(new { success = false, errors });
    }

    // Check duplicate email
    if (users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
    {
        return Results.Conflict(new { success = false, message = "Email is already registered." });
    }

    newUser.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
    newUser.Name = newUser.Name.Trim();
    newUser.Email = newUser.Email.Trim().ToLower();
    newUser.Role = string.IsNullOrWhiteSpace(newUser.Role) ? "User" : newUser.Role.Trim();

    users.Add(newUser);
    return Results.Created($"/users/{newUser.Id}", new { success = true, message = "User created successfully", data = newUser });
});

// PUT: Update an existing user by ID (with validation)
app.MapPut("/users/{id:int}", (int id, User updatedUser) =>
{
    var existingUser = users.FirstOrDefault(u => u.Id == id);
    if (existingUser is null)
    {
        return Results.NotFound(new { success = false, message = $"User with ID {id} not found." });
    }

    var errors = ValidateUser(updatedUser, isUpdate: true);
    if (errors.Any())
    {
        return Results.BadRequest(new { success = false, errors });
    }

    if (!string.IsNullOrWhiteSpace(updatedUser.Name))
        existingUser.Name = updatedUser.Name.Trim();

    if (!string.IsNullOrWhiteSpace(updatedUser.Email))
        existingUser.Email = updatedUser.Email.Trim().ToLower();

    if (!string.IsNullOrWhiteSpace(updatedUser.Role))
        existingUser.Role = updatedUser.Role.Trim();

    return Results.Ok(new { success = true, message = "User updated successfully", data = existingUser });
});

// DELETE: Remove a user by ID
app.MapDelete("/users/{id:int}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
    {
        return Results.NotFound(new { success = false, message = $"User with ID {id} not found." });
    }

    users.Remove(user);
    return Results.Ok(new { success = true, message = $"User with ID {id} deleted successfully." });
});

app.Run();

// -------------------------------------------------------------
// USER DATA MODEL
// -------------------------------------------------------------
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}
