using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DAL.DTO
{
    public class DonationsReceivedDto
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        [JsonIgnore]
        public string UserId { get; set; }
        [ForeignKey("Donation")]
        public int DonationId { get; set; }

        public string DonorName { get; set; }

        public string Category { get; set; }

        public string DonorEmail { get; set; }

        public string DonorPhone { get; set; }
    }
}
