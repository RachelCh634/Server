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
            try
            {
                var res = await _donationService.GetAllDonation();
                if (res == null)
                {
                    return BadRequest("Invalid request: Unable to retrieve donations.");
                }
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Bad Request: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching donations: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching donations.");
            }
        }

        [HttpGet("GetYourDonations")]
        public async Task<ActionResult<List<Donation>>> GetYourDonations()
        {
            try
            {
                var res = await _donationService.GetYourDonations();

                if (res == null)
                {
                    return BadRequest("Invalid request: Unable to retrieve donations.");
                }
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Bad Request: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching donations: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching donations.");
            }
        }


        [HttpGet("GetYourLikes")]
        public async Task<ActionResult<List<UserDonationLike>>> GetYourLikes()
        {
            try
            {
                var res = await _donationService.GetYourLikes();
                if (res == null)
                {
                    return BadRequest("Invalid request: Unable to retrieve donations.");
                }
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Bad Request: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching donations: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching donations.");
            }
        }

        [HttpGet("GetYourTake")]
        public async Task<ActionResult<List<DonationsReceived>>> GetYourTake()
        {
            try
            {
                var res = await _donationService.GetYourTake();
                if (res == null)
                {
                    return BadRequest("Invalid request: Unable to retrieve donations.");
                }
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Bad Request: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching donations: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching donations.");
            }
        }

        [HttpPost("AddLike/{id}")]
        public async Task<ActionResult<bool>> AddLike([FromRoute] int id)
        {
            Console.WriteLine($"donationId: {id}");
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

        [HttpGet("IsLiked/{donationId}")]
        public async Task<bool> IsLiked(int donationId)
        {
            try
            {
                return await _donationService.IsLiked(donationId);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}