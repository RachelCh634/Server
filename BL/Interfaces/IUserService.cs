using DAL.DTO;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface IUserService
    {
        public Task<List<User>> GetAllUsers();
        public Task<User> GetUserById(string id);
        public Task<bool> AddUser(UserDto user);
        public Task<bool> AddHoursDonation(int hours, string id);
        public Task<bool> RemoveHoursAvailable(int hours, string id);
    }
}
