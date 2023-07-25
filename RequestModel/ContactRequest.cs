using System.ComponentModel.DataAnnotations.Schema;

namespace RequestModels
{
    // Contact.cs (Model class)
    public class ContactRequest
    {
        [Column("user_id")]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
