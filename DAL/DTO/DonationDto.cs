using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class DonationDto
    {
        public int Id  { get; set; }
        public long DonorId { get; set; }
        public string DonationCategory { get; set; }
        public int HoursAvailable { get; set; }
        public int Rating { get; set; }
    }
}
