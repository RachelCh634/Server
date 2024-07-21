using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {   
            var res = await _userService.GetAllUsers();
            if (res.Count != 0)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var res = await _userService.GetUserById(id);
            if (res != null)
                return Ok(res);
            return BadRequest();
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult<bool>> AddUser(UserDto user)
        {
            var res = await _userService.AddUser(user);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpPost("AddHoursDonation/{hours}/{id}")]
        public async Task<ActionResult<bool>> AddHoursDonation(int hours, string id)
        {
            var res = await _userService.AddHoursDonation(hours, id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpPost("RemoveHoursAvailable/{hours}/{id}")]
        public async Task<ActionResult<bool>> RemoveHoursAvailable(int hours, string id)
        {
            var res = await _userService.RemoveHoursAvailable(hours, id);
            if (res)
                return Ok(res);
            return BadRequest();
        }
    }
}