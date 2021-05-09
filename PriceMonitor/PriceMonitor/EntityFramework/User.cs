namespace PriceMonitor.EntityFramework
{
    public class User
    {
        public string Username { get; }
        public string Email { get; set; }

        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }
}