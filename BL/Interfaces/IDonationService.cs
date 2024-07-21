using DAL.DTO;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface IDonationService
    {
        public Task<List<Donation>> GetAllDonation();
        public Task<bool> AddDonation(DonationDto donation);
        public Task<bool> DeductAvailableHours(int hours, int Id);
        public Task<bool> DeleteDonation(int Id);
        public Task<bool> RateDonation(int Id, int rating);
    }
}
