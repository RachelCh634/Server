using BL.Interfaces;
using DAL.Data;
using DAL.DTO;
using DAL.Interfaces;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class UserService : IUserService
    {
        private readonly UserData _userData;

        public UserService(UserData userData)
        {
            _userData = userData;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userData.GetAllUsers();
        }

        public async Task<User> GetUserById(string id)
        {
            return await _userData.GetUserById(id);
        }
        public async Task<object> GetUserDetails()
        {
            return await _userData.GetUserDetails();
        }
        public async Task<bool> AddUser(UserDto user)
        {
            return await _userData.AddUser(user);
        }

        public async Task<bool> AddAdmin(UserDto user)
        {
            return await _userData.AddAdmin(user);
        }

        public async Task<bool> IsUserAdmin()
        {
            return await _userData.IsUserAdmin();
        }

        public async Task<bool> AddHoursDonation(int hours, string id)
        {
            return await _userData.AddHoursDonation(hours, id);
        }

        public async Task<bool> RemoveHoursAvailable(int hours)
        {
            return await _userData.RemoveHoursAvailable(hours);
        }

        public async Task<bool> DeleteUser(string id) 
        {
            return await _userData.DeleteUser(id);
        }
        public async Task<int> CountOfHoursAvailable()
        {
            return await _userData.CountOfHoursAvailable();
        }
        public Task<string?> CurrentUserId()
        {
            return _userData.CurrentUserId();
        }
        public async Task<User?> GetCurrentUser()
        {
            return await _userData.GetCurrentUser();
        }
        public async Task<bool> UpdateUser(UserDto updatedUser)
        {
            return await _userData.UpdateUser(updatedUser);
        }

    }
}
