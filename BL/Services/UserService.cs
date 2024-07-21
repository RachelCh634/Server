using BL.Interfaces;
using DAL.Data;
using DAL.DTO;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class UserService:IUserService
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
        public async Task<bool> AddUser(UserDto user)
        {
            return await _userData.AddUser(user);
        }
        public async Task<bool> AddHoursDonation(int hours, string id)
        {
            return await _userData.AddHoursDonation(hours,id);
        }
        public async Task<bool> RemoveHoursAvailable(int hours, string id)
        {
            return await _userData.RemoveHoursAvailable(hours,id);
        }
    }
}
