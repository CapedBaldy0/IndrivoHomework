using Dapper;
using IndrivoHomework.Models;
using Microsoft.Data.SqlClient;

namespace IndrivoHomework.Data;

public class DbHelper
{
    private readonly string connectionString;
    public DbHelper(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public void AddUser(UserModel user)
    {
        var parameters = new { user.Username, user.Email, user.Password };

        var sql = "INSERT Users VALUES (@Username, @Email, @Password)";

        using var connection = new SqlConnection(connectionString);

        connection.Execute(sql, parameters);
    }

    public void SaveUser(UserModel user)
    {
        var parameters = new { user.Id, user.Username, user.Email, user.Password };

        var sql = "Update Users SET Username = @Username, Email = @Email, Password = @Password WHERE Id = @Id";

        using var connection = new SqlConnection(connectionString);

        connection.Execute(sql, parameters);
    }

    public UserModel? GetUser(string? username)
    {
        var parameters = new { Username = username};

        var sql = "SELECT * FROM Users WHERE Username = @Username";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<UserModel>(sql, parameters).FirstOrDefault();
    }

    public UserModel? GetUser(string username, string password)
    {
        var parameters = new { Username = username, Password = password };

        var sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<UserModel>(sql, parameters).FirstOrDefault();
    }

    public void SetBasicRoleToUser(int userId)
    {
        var parameters = new { UserId = userId, RoleId = 2 };

        var sql = "INSERT UserRoles VALUES (@UserId, @RoleId)";

        using var connection = new SqlConnection(connectionString);

        connection.Execute(sql, parameters);
    }

    public IEnumerable<UserModel> GetAllUsers()
    {
        var sql = "SELECT * FROM Users";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<UserModel>(sql);
    }

    public IEnumerable<UserRoleModel> GetAllUserRoles()
    {
        var sql = "SELECT UserId, Roles.Name as Role FROM UserRoles JOIN Roles ON RoleId = Roles.Id";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<UserRoleModel>(sql);
    }

    public IEnumerable<string> GetUserRoles(int userId)
    {
        var parameters = new { UserId = userId };

        var sql = "SELECT Roles.Name FROM UserRoles JOIN Roles ON RoleId = Roles.Id WHERE UserId = @UserId";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<string>(sql, parameters);
    }

    public IEnumerable<string> GetUsedUsernames()
    {
        var sql = "SELECT Username from Users";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<string>(sql);
    }

    public IEnumerable<string> GetUsedEmails()
    {
        var sql = "SELECT Email from Users";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<string>(sql);
    }

    public void ResetPassword(string email, string password)
    {
        var parameters = new { Email = email, Password = password };

        var sql = "Update Users Set Password = @Password WHERE Email = @Email";

        using var connection = new SqlConnection(connectionString);

        connection.Execute(sql, parameters);
    }

    public void LoginHistory(int userId)
    {
        var parameters = new { LoginTime = DateTime.Now, UserId = userId };

        var sql = "INSERT LoginHistory VALUES (@LoginTime, @UserId)";

        using var connection = new SqlConnection(connectionString);

        connection.Execute(sql, parameters);
    }

    public IEnumerable<LoginHistoryModel> GetLoginHistory()
    {
        var sql = "SELECT LoginTime as 'Time', Users.Username, Users.Email FROM LoginHistory JOIN Users ON UserId = Users.Id ORDER BY Time DESC";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<LoginHistoryModel>(sql);
    }
}
