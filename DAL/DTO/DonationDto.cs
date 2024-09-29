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
    public class DonationDto
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public int Id  { get; set; }
        [Required]
        public long DonorId { get; set; }
        [Required]
        public string DonationCategory { get; set; }
        [Required]
        public int HoursAvailable { get; set; }
        [JsonIgnore]
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int Rating { get; set; }
        [JsonIgnore]
        public int CountRate { get; set; } = 0;
        public string Description { get; set; }
        [JsonIgnore]
        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public DateTime Date { get; set; }
    }
}
