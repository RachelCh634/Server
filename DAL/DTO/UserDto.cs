using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class UserDto
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public int IdentityId { get; set; }
        [Required]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public int HoursDonation { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public int HoursAvailable { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public string Role { get; set; } = "User";
        public string Phone { get; set; }
        public string City { get; set; }
    }
}
