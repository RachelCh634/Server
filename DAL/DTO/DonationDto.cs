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
        [SwaggerSchema(ReadOnly = true)]
        public int Rating { get; set; }
    }
}
