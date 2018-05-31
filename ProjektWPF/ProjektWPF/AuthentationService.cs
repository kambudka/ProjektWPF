using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ProjektWPF
{
    public interface IAuthenticationService
    {
        User AuthenticateUser(string username, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private class InternalUserData
        {
            public InternalUserData(string username, string email, string hashedPassword, string[] roles)
            {
                Username = username;
                Email = email;
                HashedPassword = hashedPassword;
                Roles = roles;
            }
            public string Username
            {
                get;
                private set;
            }

            public string Email
            {
                get;
                private set;
            }

            public string HashedPassword
            {
                get;
                private set;
            }

            public string[] Roles
            {
                get;
                private set;
            }
        }

        private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        {
            new InternalUserData("ksiegowy", "mark@company.com",
            "ksiegowy", new string[] { "Ksiegowy" }),
            new InternalUserData("kurier", "john@company.com",
            "kurier", new string[] { "Kurier" }),
            new InternalUserData("kurier2", "john@company.com",
            "kurier", new string[] { "Kurier" }),
            new InternalUserData("admin", "admin@company.com",
            "admin", new string[] { "Administrators" })
        };

        public User AuthenticateUser(string username, string clearTextPassword)
        {
            InternalUserData userData = _users.FirstOrDefault(u => u.Username.Equals(username)
                && u.HashedPassword.Equals(clearTextPassword));
            if (userData == null)
                throw new UnauthorizedAccessException("Złe hasło lub login");

            return new User(userData.Username, userData.Email, userData.Roles);
        }
    }

    public class User
    {
        public User(string username, string email, string[] roles)
        {
            Username = username;
            Email = email;
            Roles = roles;
        }
        public string Username
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string[] Roles
        {
            get;
            set;
        }
    }
}