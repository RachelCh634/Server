using DAL.DTO;
using MODELS.Models;
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
        public Task<List<Donation>> GetYourDonations();
        public Task<List<UserDonationLike>> GetYourLikes();
        public Task<List<DonationsReceived>> GetYourTake();
        public Task<bool> AddLike(int donationId);
        public Task<bool> AddDonation(DonationDto donation);
        public Task<bool> DeductAvailableHours(int hours, int Id);
        public Task<bool> DeleteDonation(int Id);
        public Task<bool> RateDonation(int Id, int rating);
    }
}
