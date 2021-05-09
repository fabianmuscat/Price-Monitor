using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PriceMonitor.EntityFramework
{
    public class UserAccount
    {
        public static User LoggedUser { get; private set;  }
    
        public bool Login(string username, string password)
        {
            string encrypted = Encrypt(password);
            string email = string.Empty;
            bool loggedIn = false;
    
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Account account = db.Accounts.FirstOrDefault(ac => ac.Username.Equals(username) && ac.Password.Equals(encrypted));
                if (account != null)
                {
                    email = (account.Email != null) ? account.Email : null;
                    LoggedUser = (email != null) ? new User(username, email) : new User(username, null);
                    loggedIn = true;
                }
                else LoggedUser = null;
            }
    
            return loggedIn;
        }
    
        public bool CreateAccount(string username, string password, string email = null)
        {
            string encrypted = Encrypt(password);
            bool created = false;

            try
            {
                using (PriceMonitorEntities db = new PriceMonitorEntities())
                {
                    if (!db.Accounts.Any(ac => ac.Username.Equals(username)))
                    {
                        Account account = new Account();
                        account.Username = username;
                        account.Password = encrypted;
                        account.Email = email;

                        db.Accounts.Add(account);
                        db.SaveChanges();
                        created = true;
                    }
                }
            }
            catch { }
            return created;
        }
    
        private string Encrypt(string password)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(password));
                return Convert.ToBase64String(data);
            }
        }

        public static void Logout() => LoggedUser = null;
    }
}