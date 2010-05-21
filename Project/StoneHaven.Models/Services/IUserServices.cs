namespace StoneHaven.Models.Services
{
    public interface IUserServices
    {
        bool Authenticate(string username, string password);
        User GetUser(string username);
    }
}