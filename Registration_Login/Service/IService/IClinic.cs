using Registration_Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration_Login.Service.IService
{
  public  interface IClinic
    {
        Task<Result> GetClinics();
        Task<Result> AddClinic(Clinic clinic);
        Task<Result> DeleteClinic(int? clinicId);
        Task<Result> UpdateClinic(Clinic clinic);
        Task Save();
        Task<Result> GetClinic(int clinicId);
    }
}
