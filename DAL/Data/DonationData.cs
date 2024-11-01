using AutoMapper;
using DAL.DTO;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MODELS.Models;
using Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace DAL.Data
{
    public class DonationData : IDonationData
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserData _userData;
        private readonly ILogger<DonationData> _logger;
        public DonationData(DBContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserData userData, ILogger<DonationData> logger)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userData = userData;
            _logger = logger;
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
            var userIdInt = int.Parse(userId);
            var donations = await _context.Donations
                .Where(d => d.DonorId == userIdInt)
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
            var donations = await _context.DonationsReceiveds
                .Where(d => d.UserId == userId)
                .ToListAsync();
            return donations;
        }
        public async Task<bool> DeductAvailableHours(int hours, int Id)
        {
            try
            {
                _logger.LogInformation($"Starting DeductAvailableHours with Id: {Id} and hours: {hours}");

                var donation = await _context.Donations.FindAsync(Id);
                if (donation == null)
                {
                    _logger.LogWarning($"Donation with Id: {Id} not found.");
                    return false;
                }

                if (donation.HoursAvailable <= 0)
                {
                    _logger.LogInformation($"Donation with Id: {Id} has no available hours.");
                    donation.HoursAvailable = 0;
                    donation.IsActive = false;
                }
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (userId == null)
                {
                    _logger.LogWarning("User not authenticated.");
                    return false;
                }

                var user = await _context.Users.FindAsync(userId.PadLeft(9, '0'));
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                    return false;
                }

                var donor = await _context.Users.FindAsync(donation.DonorId.ToString().PadLeft(9, '0'));
                if (donor == null)
                {
                    _logger.LogWarning($"Donor not found for Donation ID: {Id}");
                    return false;
                }

                var existingDonation = _context.DonationsReceiveds.FirstOrDefault(d => d.DonationId == Id && d.UserId == user.Id);
                if (existingDonation != null)
                {
                    existingDonation.Hours += hours;
                    _logger.LogInformation($"Updated existing donation received for Donation ID: {Id}");
                }
                else
                {
                    var newDonationReceived = new DonationsReceived
                    {
                        UserId = userId.ToString(),
                        Hours = hours,
                        DonationId = Id,
                        DonorName = donor.FirstName + " " + donor.LastName,
                        Category = donation.DonationCategory,
                        DonorEmail = donor.Email,
                        DonorPhone = donor.Phone,
                    };
                    _context.DonationsReceiveds.Add(newDonationReceived);
                    _logger.LogInformation($"Added new donation received for Donation ID: {Id}");
                }

                donation.HoursAvailable -= hours;
                bool RemoveHours = await _userData.RemoveHoursAvailable(hours);
                if (!RemoveHours)
                {
                    _logger.LogWarning("Failed to remove hours available for user ID: {UserId}", userId);
                    return false;
                }
                try
                {
                    _context.Entry(donation).State = EntityState.Modified;
                    int changes = await _context.SaveChangesAsync();
                    _logger.LogInformation($"Changes saved: {changes}");
                    return changes > 0;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database update error occurred.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deducting available hours.");
                return false;
            }
        }
        public async Task<bool> AddLike(int donationId)
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userId == null)
            {
                return false;
            }
            Console.WriteLine($"UserId: {userId}");
            var existingLike = await _context.UserDonationLikes
                .FirstOrDefaultAsync(like => like.DonationId == donationId && like.UserId == userId);

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
            string idString = donation.DonorId.ToString().PadLeft(9, '0');
            var user = await _context.Users.FindAsync(idString);
            if (user == null)
            {
                return false;
            }
            user.HoursAvailable -= donation.HoursAvailable;
            user.HoursDonation -= donation.HoursAvailable;
            _context.Donations.Remove(donation);
            var likes = _context.UserDonationLikes.Where(like => like.DonationId == Id);
            _context.UserDonationLikes.RemoveRange(likes);
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool> RateDonation(int Id, int rating)
        {
            var donationReceived = await _context.DonationsReceiveds.FindAsync(Id);
            if (donationReceived == null) { return false; }

            var donation = await _context.Donations.FindAsync(donationReceived.DonationId);
            if (donation != null)
            {
                int previousRating = donationReceived.Rating;

                int totalRating = donation.Rating * donation.CountRate;

                totalRating = totalRating - previousRating + rating;

                if (previousRating == 0)
                {
                    donation.CountRate += 1;
                }
                if (donation.CountRate > 0)
                {
                    donation.Rating = totalRating / donation.CountRate;
                }
                else
                {
                    donation.Rating = 0;
                }
            }
            donationReceived.Rating = rating;
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
