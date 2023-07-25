
namespace RequestModels
{
    // Contact.cs (Model class)
    public class Contact: ContactRequest
    {
        public int Id { get; set; }

        // Navigation property for User
        public User User { get; set; }

        public Contact() { }

        public Contact(ContactRequest contactRequest)
        {
            this.Email = contactRequest.Email;
            this.Phone = contactRequest.Phone; 
            this.Name = contactRequest.Name;
            this.UserId = contactRequest.UserId;

            //TODO we might need to retrive the user from the userId and assign it to the User navigation property 
        }
    }
}
