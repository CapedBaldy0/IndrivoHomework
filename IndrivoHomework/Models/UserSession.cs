namespace IndrivoHomework.Models
{
    public class UserSession
    {
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
