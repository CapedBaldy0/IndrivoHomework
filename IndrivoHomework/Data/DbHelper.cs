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
        //var parameters = new { Username = user.Username, Email = user.Email, Password = user.Password };
        var parameters = new { user.Username, user.Email, user.Password };

        var sql = "INSERT Users VALUES (@Username, @Email, @Password)";

        using var connection = new SqlConnection(connectionString);

        connection.Execute(sql, parameters);
    }

    public UserModel? GetUser(string username, string password)
    {
        var parameters = new { Username = username, Password = password };

        var sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";

        using var connection = new SqlConnection(connectionString);

        return connection.Query<UserModel>(sql, parameters).FirstOrDefault();
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
}
