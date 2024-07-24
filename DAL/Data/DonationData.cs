using AutoMapper;
using DAL.DTO;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using MODELS.Models;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Data
{
    public class DonationData : IDonationData
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        public DonationData(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddDonation(DonationDto donation)
        {
            _context.Donations.Add(_mapper.Map<Donation>(donation));
            string idString = donation.DonorId.ToString();
            var user = await _context.Users.FindAsync(idString);
            if (user == null)
            {
                return false; 
            }
            user.HoursAvailable += donation.HoursAvailable;
            user.HoursDonation += donation.HoursAvailable;
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<List<Donation>> GetAllDonation()
        {
            var users = await _context.Donations.ToListAsync();
            return users;
        }

        public async Task<bool> DeductAvailableHours(int hours, int Id)
        {
            var donation = await _context.Donations.FindAsync(Id);
            if (donation == null)
            {
                return false; 
            }

            donation.HoursAvailable -= hours;
            if (donation.HoursAvailable <= 0)
            {
                return await DeleteDonation(Id);
            }

            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> DeleteDonation(int Id)
        {
            var donation = await _context.Donations.FindAsync(Id);
            if (donation == null)
            {
                return false; 
            }
            string idString = donation.DonorId.ToString();
            var user = await _context.Users.FindAsync(idString);
            if (user == null)
            {
                return false;
            }
            user.HoursAvailable -= donation.HoursAvailable;
            user.HoursDonation -= donation.HoursAvailable;
            _context.Donations.Remove(donation);
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> RateDonation(int Id, int rating)
        {
            var donation = await _context.Donations.FindAsync(Id);
            if (donation == null)
            {
                return false; 
            }
            donation.Rating = rating;
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}
