using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using MODELS.Models;

namespace Project 
{
    public class User
    {
       
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdentityId { get; set; }
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int HoursDonation { get; set; }
        public int HoursAvailable { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }

        public User(string id, string firstName, string lastName, string email,
            int hoursDonation, int hoursAvailable, string role,string phone,string city)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            HoursDonation = hoursDonation;
            HoursAvailable = hoursAvailable;
            Role = role;
            Phone = phone;
            City = city;
        }
    }
}
