namespace ContactsApi.Models
{
    using RequestModels;
    using System.ComponentModel.DataAnnotations;

    public class Session
    {
        [Key]
        public string Token { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
