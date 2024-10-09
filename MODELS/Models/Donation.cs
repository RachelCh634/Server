using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project
{
    public class Donation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public long DonorId { get; set; }
        public string DonationCategory { get; set; }
        public int HoursAvailable { get; set; }
        public int OriginalHours { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime Date { get; set; }

        public Donation(int id, long donorId, string donationCategory, int hoursAvailable, int rating, string description)
        {
            Id = id;
            DonorId = donorId;
            DonationCategory = donationCategory;
            HoursAvailable = hoursAvailable;
            OriginalHours = hoursAvailable;
            Rating = rating;
            Description = description;
            IsActive = true;
            Date= DateTime.Now;
        }

        public Donation() { }
    }
}
