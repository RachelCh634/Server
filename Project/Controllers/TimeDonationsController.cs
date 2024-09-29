using BL.Interfaces;
using DAL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MODELS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class TimeDonationsController : Controller
    {
        private readonly IDonationService _donationService;

        public TimeDonationsController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpPost("AddDonation")]
        [Authorize]
        public async Task<ActionResult<bool>> AddDonation([FromBody] DonationDto donation)
        {
            var res = await _donationService.AddDonation(donation);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("GetAllDonations")]
        public async Task<ActionResult<List<Donation>>> GetAllDonations()
        {
            var res = await _donationService.GetAllDonation();
            if (res.Count != 0)
                return Ok(res);
            return BadRequest();
        }
        [HttpGet("GetYourDonations")]
        public async Task<ActionResult<List<Donation>>> GetYourDonations()
        {
            var res = await _donationService.GetYourDonations();
            if (res.Count != 0)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("GetYourLikes")]
        public async Task<ActionResult<List<UserDonationLike>>> GetYourLikes()
        {
            var res = await _donationService.GetYourLikes();
            if (res.Count != 0)
                return Ok(res);
            return BadRequest();
        }

        [HttpGet("GetYourTake")]
        public async Task<ActionResult<List<DonationsReceived>>> GetYourTake()
        {
            var res = await _donationService.GetYourTake();
            if (res.Count != 0)
                return Ok(res);
            return BadRequest();
        }

        [HttpPost("AddLike")]
        public async Task<ActionResult<bool>> AddLike(int id)
        {
            Console.WriteLine("donationId", id);
            var res = await _donationService.AddLike(id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpPut("DeductAvailableHours")]
        public async Task<ActionResult<bool>> DeductAvailableHours(int hours, int Id)
        {
            var res = await _donationService.DeductAvailableHours(hours, Id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpDelete("DeleteDonation/{id}")]
        public async Task<ActionResult<bool>> DeleteDonation([FromRoute] int id)
        {
            var res = await _donationService.DeleteDonation(id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpPut("RateDonation")]
        public async Task<ActionResult<bool>> RateDonation([FromQuery] int Id, [FromQuery] int rating)
        {
            var res = await _donationService.RateDonation(Id, rating);
            if (res)
                return Ok(res);
            return BadRequest();
        }
    }
}