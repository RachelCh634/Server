using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project 
{
    public class User
    {
       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int IdentityId { get; set; }
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int HoursDonation { get; set; }
        public int HoursAvailable { get; set; } 
        public User(string id, string firstName, string lastName, string email, string address, int hoursDonation, int hoursAvailable)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Address = address;
            HoursDonation = hoursDonation;
            HoursAvailable = hoursAvailable;
        }
    }
}
