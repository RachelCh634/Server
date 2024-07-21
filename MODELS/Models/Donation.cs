using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project
{
    public class Donation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public long DonorId { get; set; }
        public string DonationCategory { get; set; }
        public int HoursAvailable { get; set; }
        public int Rating { get; set; } 

        public Donation(int id, long donorId, string donationCategory, int hoursAvailable, int rating)
        {
            Id = id;
            DonorId = donorId;
            DonationCategory = donationCategory;
            HoursAvailable = hoursAvailable;
            Rating = rating;
        }

        public Donation() { }
    }
}
