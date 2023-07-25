using System.Collections.Generic;

namespace RequestModels
{
    public class User : UserRequest
    {
        public int Id { get; set; }

        // Navigation property for Contacts
        public ICollection<Contact> Contacts { get; set; }

        public User() { }
        public User(UserRequest ur)
        {
            this.Username = ur.Username;
            this.Password = ur.Password;
        }


    }

}
