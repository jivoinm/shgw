using NHibernate.Criterion;
using Query;
using Rhino.Commons;

namespace StoneHaven.Models.Services.DB
{
    public class UserServices : IUserServices
    {
        public bool Authenticate(string username, string password)
        {
            return Repository<User>.FindOne(Restrictions.Eq("UserName", username), Restrictions.Eq("Password", password)) != null;
        }

        public User GetUser(string username )
        {
            return Repository<User>.FindFirst(Where.User.UserName==username);
        }
    }
}