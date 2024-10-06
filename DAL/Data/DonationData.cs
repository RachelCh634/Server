using AutoMapper;
using DAL.DTO;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MODELS.Models;
using Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DAL.Data
{
    public class DonationData : IDonationData
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DonationData(DBContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<bool> AddDonation(DonationDto donation)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID is null or empty.");
                return false;
            }
            Console.WriteLine($"User ID: {userId}");

            var donationEntity = _mapper.Map<Donation>(donation);
            donationEntity.DonorId = long.Parse(userId);

            Console.WriteLine($"Donation Entity: {donationEntity}");

            _context.Donations.Add(donationEntity);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return false;
            }

            user.HoursAvailable += donation.HoursAvailable;
            user.HoursDonation += donation.HoursAvailable;

            try
            {
                int changes = await _context.SaveChangesAsync();
                return changes > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Donation>> GetAllDonation()
        {
            var users = await _context.Donations.ToListAsync();
            return users;
        }
        public async Task<List<Donation>> GetYourDonations()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userId == null)
            {
                return new List<Donation>(); 
            }

            var donations = await _context.Donations
                .Where(d => d.DonorId.ToString() == userId)
                .ToListAsync();

            return donations;
        }
        public async Task<List<UserDonationLike>> GetYourLikes()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userId == null)
            {
                Console.WriteLine("Error");
                return new List<UserDonationLike>();
            }
            var donations = await _context.UserDonationLikes
                .Where(d => d.UserId == userId)
                .ToListAsync();
            return donations;
        }
        public async Task<List<DonationsReceived>> GetYourTake()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                Console.WriteLine("null Id");
                return new List<DonationsReceived>();
            }
            Console.WriteLine("good Id");

            var donations = await _context.DonationsReceiveds
                .Where(d => d.UserId == userId)
                .ToListAsync();
            return donations;
        }
        public async Task<bool> DeductAvailableHours(int hours, int Id)
        {
            var donation = await _context.Donations.FindAsync(Id);
            if (donation == null)
            {
                return false;
            }

            donation.HoursAvailable -= hours;

            if (donation.HoursAvailable <= 0)
            {
                donation.HoursAvailable = 0;
                donation.IsActive = false;
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userId == null)
            {
                Console.WriteLine("userId == null, Token is missing or invalid");
                var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    Console.WriteLine("Claims: ");
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                    }
                }
                return false;
            }

            var existingDonation = _context.DonationsReceiveds.FirstOrDefault(d => d.DonationId == Id);
            if (existingDonation != null)
            {
                existingDonation.Hours += hours;
            }
            else
            {
                var newDonationReceived = new DonationsReceived
                {
                    UserId = userId.ToString(),
                    Hours = hours,
                    DonationId = Id
                };

                _context.DonationsReceiveds.Add(newDonationReceived);
            }

            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }
        public async Task<bool> AddLike(int donationId)
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                return false;
            }

            var existingLike = await _context.UserDonationLikes
                .FirstOrDefaultAsync(like => like.DonationId == donationId);

            if (existingLike != null)
            {
                _context.UserDonationLikes.Remove(existingLike);
            }
            else
            {
                var newDonationLike = new UserDonationLike
                {
                    UserId = userId.ToString(),
                    DonationId = donationId
                };
                _context.UserDonationLikes.Add(newDonationLike);
            }
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool> DeleteDonation(int Id)
        {
            var donation = await _context.Donations.FindAsync(Id);
            if (donation == null)
            {
                return false;
            }
            string idString = donation.DonorId.ToString();
            var user = await _context.Users.FindAsync(idString);
            if (user == null)
            {
                return false;
            }
            user.HoursAvailable -= donation.HoursAvailable;
            user.HoursDonation -= donation.HoursAvailable;
            donation.IsActive = false;
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool> RateDonation(int Id, int rating)
        {
            var donation = await _context.Donations.FindAsync(Id);
            if (donation == null)
            {
                return false;
            }
            donation.Rating = rating;
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool> IsLiked(int donationId)
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                return false;
            }
            var donationLike = await _context.UserDonationLikes
                .FirstOrDefaultAsync(like => like.UserId == userId && like.DonationId == donationId);
            return donationLike != null;
        }

    }
}
