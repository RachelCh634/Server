using BL.Interfaces;
using DAL.Data;
using DAL.DTO;
using DAL.Interfaces;
using MODELS.Models;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class DonationService:IDonationService
    {
        private readonly DonationData _donationData;
        public DonationService(DonationData donationData)
        {
            _donationData = donationData;
        }
        public async Task<bool> AddDonation(DonationDto donation)
        {
            return await _donationData.AddDonation(donation);
        }

        public async Task<List<Donation>> GetAllDonation()
        {
            return await _donationData.GetAllDonation();
        }

        public async Task<List<Donation>> GetYourDonations()
        {
            return await _donationData.GetYourDonations();
        }

        public async Task<List<UserDonationLike>> GetYourLikes()
        {
            return await _donationData.GetYourLikes();
        }
        public async Task<List<DonationsReceived>> GetYourTake()
        {
            return await _donationData.GetYourTake();
        }

        public async Task<bool> AddLike(int donationId)
        {
            return await _donationData.AddLike(donationId);
        }
        public async Task<bool> DeductAvailableHours(int hours, int Id)
        {
            return await _donationData.DeductAvailableHours(hours,Id);
        }

        public async Task<bool> DeleteDonation(int Id)
        {
            return await _donationData.DeleteDonation(Id);
        }

        public async Task<bool> RateDonation(int Id, int rating)
        {
            return await _donationData.RateDonation(Id,rating);
        }

        public async Task<bool> IsLiked(int donationId)
        {
            return await _donationData.IsLiked(donationId);
        }
    }
}
