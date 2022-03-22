using System;
using System.Collections.Generic;

namespace SuperPanel.App.Models
{
    public class UserView
    {
        public int Id { get; set; }

        public string Login { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Checked { get; set; }
        public UserView() { }

        public UserView(User user)
        {
            this.Id = user.Id;
            this.Login = user.Login;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Phone = user.Phone;
            this.CreatedAt = user.CreatedAt;
            this.Checked = false;
        }
    }
}
