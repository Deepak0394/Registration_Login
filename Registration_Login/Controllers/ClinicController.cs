using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Registration_Login.Models;
using Registration_Login.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration_Login.Controllers
{
    [Route("api/Clinic")]
    [ApiController]
    public class ClinicController : ControllerBase
    {
        private readonly IClinic _clinicService;
        public ClinicController(IClinic clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet("ClinicList")]
        public async Task<ActionResult<Result>> GetClinics()
        {
            return await _clinicService.GetClinics();
        }

        [HttpGet("{Id:int}", Name = "GetClinic")]
        public async Task<ActionResult<Result>> GetClinic(int Id)
        {
            return await _clinicService.GetClinic(Id);
        }

        [HttpPost("SaveClinic")]
        public async Task<ActionResult<Result>> AddClinic(Clinic clinic)
        {
            if (clinic == null)
            {
                return Result.Failure(new string[] { "Data not Found" });
            }
            if (!ModelState.IsValid)
            {
                return Result.Failure(new string[] { "Enter Valid Data" });
            }
            return await _clinicService.AddClinic(clinic);

        }

        [HttpDelete("{clinicId}")]
        public async Task<ActionResult<Result>> DeleteClinic(int? clinicId)
        {
            if (await _clinicService.DeleteClinic(clinicId) == null)
            {
                return Result.Failure(new string[] { "User doesnt found" });
            }
            return Result.Success(new string[] { "Data Deleted successfully" });
        }
        [HttpPut("UpdateClinic")]
        public async Task<ActionResult<Result>> UpdateClinic(Clinic clinic)
        {
            if (clinic == null)
            {
                return Result.Failure(new string[] { "Data not Found" });
            }
            if (!ModelState.IsValid)
            {
                return Result.Failure(new string[] { "Enter Valid Data" });
            }
            return Result.Success(await _clinicService.UpdateClinic(clinic));
        }
    }
}
