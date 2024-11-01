using AutoMapper;
using DAL.DTO;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MODELS.Models;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class UserData : IUserData
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserData(DBContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<object> GetUserDetails()
        {
            var Id = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var user = await _context.Users.FindAsync(Id);
            var fullName = $"{user?.FirstName} {user?.LastName}";
            var details = new
            {
                fullName,
                user?.Role,
                user?.Email
            };
            return details;
        }
        public async Task<bool> AddUser(UserDto userDto)
        {
            string userId = userDto.Id;

            if (!IsValidIsraeliId(userId))
            {
                return false;
            }

            string role = !_context.Users.Any() ? "Admin" : "User";

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.Role = role;

            _context.Users.Add(userEntity);

            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> AddAdmin(UserDto userDto)
        {
            string userId = userDto.Id;

            if (!IsValidIsraeliId(userId))
            {
                return false;
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.Role = "Admin";

            _context.Users.Add(userEntity);

            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> IsUserAdmin()
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var user = await _context.Users.FindAsync(id);
            return user != null && user.Role == "Admin";
        }

        private bool IsValidIsraeliId(string id)
        {
            if (id.Length != 9 || !id.All(char.IsDigit))
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int num = int.Parse(id[i].ToString());
                int weight = i % 2 == 0 ? num : num * 2;

                sum += weight > 9 ? weight - 9 : weight;
            }

            return sum % 10 == 0;
        }

        public async Task<bool> AddHoursDonation(int hours, string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            user.HoursDonation += hours;
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<bool> RemoveHoursAvailable(int hours)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User
                    .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (userId == null)
                {
                    Console.WriteLine("User ID not found in claims.");
                    return false;
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {userId} not found.");
                    return false;
                }

                if ((user.HoursAvailable - hours) < 0)
                {
                    Console.WriteLine("Not enough available hours.");
                    return false;
                }

                user.HoursAvailable -= hours;
                int changes = await _context.SaveChangesAsync();
                return changes > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            var donations = _context.Donations.Where(d => d.DonorId.ToString() == id).ToList();

            var donationIds = donations.Select(d => d.Id).ToList();
            var donationsToDelete = _context.UserDonationLikes
                .Where(d => donationIds.Contains(d.DonationId))
                .ToList();

            if (donationsToDelete.Any())
            {
                _context.UserDonationLikes.RemoveRange(donationsToDelete);
            }

            var donationsReceived = await _context.DonationsReceiveds
                .Where(d => d.UserId == id).ToListAsync();
            _context.DonationsReceiveds.RemoveRange(donationsReceived);

            var donationsLikes = await _context.UserDonationLikes
             .Where(d => d.UserId == id).ToListAsync();
            _context.UserDonationLikes.RemoveRange(donationsLikes);

            _context.Donations.RemoveRange(donations);

            _context.Users.Remove(user);

            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
        public async Task<int> CountOfHoursAvailable()
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                return user.HoursAvailable;
            }
            return -1;
        }
        public Task<string?> CurrentUserId()
        {
            var Id = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            return Task.FromResult(Id);
        }
        public async Task<User?> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID is null or empty.");
                return null;
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"User not found for ID: {userId}");
            }
            return user;
        }
        public async Task<bool> UpdateUser(UserDto updatedUser)
        {
            if (updatedUser == null)
            {
                throw new ArgumentNullException(nameof(updatedUser), "The updated user cannot be null.");
            }
            var existingUser = await _context.Users.FindAsync(updatedUser.Id);

            if (existingUser == null)
            {
                Console.WriteLine($"User not found for ID: {updatedUser.Id}");
                return false; 
            }

            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.Phone = updatedUser.Phone;
            existingUser.City = updatedUser.City;

            await _context.SaveChangesAsync();
            return true; 
        }
    }
}
