using JewelryPOS.App.Models;
using System;

namespace JewelryPOS.App.Helpers
{
    public class UserSession
    {
        private static UserSession _instance;
        private static readonly object _lock = new object();

        public ApplicationUser CurrentUser { get; private set; }
        public event EventHandler UserChanged;

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new UserSession();
                }
            }
        }

        public void SetUser(ApplicationUser user)
        {
            CurrentUser = user;
            UserChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearUser()
        {
            CurrentUser = null;
            UserChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}