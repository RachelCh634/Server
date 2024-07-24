using DAL.DTO;
using Project;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(string id);
        Task<bool> AddUser(UserDto user);
        Task<bool> AddAdmin(UserDto user);
        Task<bool> IsUserAdmin(string id);
        Task<bool> AddHoursDonation(int hours, string id);
        Task<bool> RemoveHoursAvailable(int hours, string id);
        Task<bool> DeleteUser(string id);
    }
}