using Microsoft.EntityFrameworkCore;
using Registration_Login.Data;
using Registration_Login.Models;
using Registration_Login.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration_Login.Service
{
    public class ClinicService : IClinic
    {
        private readonly ApplicationDbContext _context;
        public ClinicService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result> AddClinic(Clinic clinic)
        {
            await _context.Clinics.AddAsync(clinic);
            await Save();
            return Result.Success();
        }

        public async Task<Result> DeleteClinic(int? clinicId)
        {
            var ClinicInDb = await _context.Clinics.FindAsync(clinicId);
            if (ClinicInDb == null)
            {
                Result.Failure(new String[] { "ID not found" });
            }
            else
                _context.Clinics.Remove(ClinicInDb);
            await Save();

            return Result.Success();
        }

        public async Task<Result> GetClinic(int clinicId)
        {
            var obj = await _context.Clinics.FindAsync(clinicId);
            if (obj != null)
            {
                return Result.Success(obj);
            }
            else
                return Result.Failure(new string[] { "Hospital not found" });
        }

        public async Task<Result> GetClinics()
        {
            var obj = await _context.Clinics.ToListAsync();
            return Result.Success(obj);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Result> UpdateClinic(Clinic clinic)
        {
            var obj = await _context.Clinics.FindAsync(clinic.Id);
            if (obj == null)
                return Result.Failure(new string[] { "Hospital not found" });
            else
                obj.Name = clinic.Name;
            obj.Address = clinic.Address;
            obj.Email = clinic.Email;
            obj.PhoneNo = clinic.PhoneNo;
            obj.EmergencyNo = clinic.EmergencyNo;
            obj.Facilities = clinic.Facilities;
            obj.doctorlistId = clinic.doctorlistId;
            _context.Clinics.Update(obj);
            await Save();
            return Result.Success();
        }
    }
}
