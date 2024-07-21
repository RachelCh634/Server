using BL.Interfaces;
using DAL.DTO;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("DeductAvailableHours")]
        public async Task<ActionResult<bool>> DeductAvailableHours([FromQuery] int hours, [FromQuery] int Id)
        {
            var res = await _donationService.DeductAvailableHours(hours, Id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpDelete("DeleteDonation")]
        public async Task<ActionResult<bool>> DeleteDonation([FromQuery] int Id)
        {
            var res = await _donationService.DeleteDonation(Id);
            if (res)
                return Ok(res);
            return BadRequest();
        }

        [HttpPost("RateDonation")]
        public async Task<ActionResult<bool>> RateDonation([FromQuery] int Id, [FromQuery] int rating)
        {
            var res = await _donationService.RateDonation(Id, rating);
            if (res)
                return Ok(res);
            return BadRequest();
        }
    }
}