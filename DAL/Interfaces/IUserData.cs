using DAL.DTO;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserData
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(string id);
        Task<object> GetUserDetails();
        Task<bool> AddUser(UserDto user);
        Task<bool> AddAdmin(UserDto user);
        Task<bool> IsUserAdmin();
        Task<bool> AddHoursDonation(int hours, string id);
        Task<bool> RemoveHoursAvailable(int hours, string id);
        Task<bool> DeleteUser(string id); 
    }
}
