using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using DAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Project.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        [HttpGet("GetUserDetails")]

        public async Task<ActionResult<object>> GetUserDetails()
        {
            var res = await _userService.GetUserDetails();
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

        [HttpPost("AddAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> AddAdmin(UserDto user)
        {
            var res = await _userService.AddAdmin(user);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("IsUserAdmin")]
        public async Task<ActionResult<bool>> IsUserAdmin()
        {
            var res = await _userService.IsUserAdmin();
            return Ok(res);
        }

        [HttpPost("AddHoursDonation/{hours}/{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> AddHoursDonation(int hours, string id)
        {
            var res = await _userService.AddHoursDonation(hours, id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpPut("RemoveHoursAvailable/{hours}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveHoursAvailable(int hours)
        {
            var res = await _userService.RemoveHoursAvailable(hours);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<bool>> DeleteUser(string id)
        {
            var res = await _userService.DeleteUser(id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("CountOfHoursAvailable")]
        public async Task<ActionResult<int>> CountOfHoursAvailable()
        {
            var res = await _userService.CountOfHoursAvailable();
            if (res != -1)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("CurrentUserId")]
        public async Task<ActionResult<string?>> CurrentUserId()
        {
            var res = await _userService.CurrentUserId();
            if (res != null)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<User?>> GetCurrentUser()
        {
            var res = await _userService.GetCurrentUser();
            if (res != null)
                return Ok(res);
            return BadRequest();
        }
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<bool>> UpdateUser(UserDto updatedUser)
        {
            var res = await _userService.UpdateUser(updatedUser);
            if (res)
                return Ok(res);
            return BadRequest();
        }
    }
}
