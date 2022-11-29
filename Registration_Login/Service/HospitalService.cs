using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Registration_Login.Data;
using Registration_Login.Identity;
using Registration_Login.Models;
using Registration_Login.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Registration_Login.Service
{
    public class HospitalService:IHospitalService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSetting _mailSettings;
        private readonly UserManager<ApplicationUser> _userManager;
    
        public HospitalService(ApplicationDbContext context, IOptions<AppSetting> mailSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mailSettings = mailSettings.Value;
            _userManager = userManager;
          
        }

      
        public async Task<Result> DeleteHospital(int? hospitalId)
        {
            var hospitalInDb = await _context.hospital.FindAsync(hospitalId);
            if (hospitalInDb == null)
            {
                Result.Failure(new String[] { "ID not found" });
            }
            else
                _context.hospital.Remove(hospitalInDb);
            await Save();

            return Result.Success();
        }

        public async Task<Result> GetHospital(int hospitalId)
        {
            var obj = await _context.hospital.FindAsync(hospitalId);
            if (obj != null)
            {
                return Result.Success(obj);
            }
            else
                return Result.Failure(new string[] { "Hospital not found" });
        }

        public async Task<Result> GetHospitals()
        {
            var obj = await _context.hospital.ToListAsync();
            return Result.Success(obj);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();

        }
    

        public async Task<Result> UpdateHospital(hospitals hospitals)
        {
            var obj = await _context.hospital.FindAsync(hospitals.Id);
            if (obj == null)
                return Result.Failure(new string[] { "Hospital not found" });
            else
                obj.hospitalname = hospitals.hospitalname;
            obj.facilities = hospitals.facilities;
            obj.department = hospitals.department;
            ; 
            _context.hospital.Update(obj);
            await Save();

            return Result.Success();
        }
        public async Task<hospitals> AddHospital(hospitals hospitals)
        {

            var userEmailAlreadyExists = _context.hospital.FirstOrDefault(x => x.Email == hospitals.Email );
            var userNameAlreadyExists = _context.hospital.FirstOrDefault(x => x.UserName == hospitals.UserName);
            var userPhoneAlreadyExists = _context.hospital.FirstOrDefault(x => x.PhoneNumber == hospitals.PhoneNumber);
            if(userEmailAlreadyExists!=null)
            {
                return userEmailAlreadyExists;
            }
       
            else if(userNameAlreadyExists!=null)
            {
                return userNameAlreadyExists;
            }
            else if(userPhoneAlreadyExists!=null)
            {
                return userPhoneAlreadyExists;
            }
           else      
            await _context.hospital.AddAsync(hospitals);
            await Save();         
            //Random Password Genrated 
            string strNewPassword =HospitalService.GenerateRandomPassword();
            //Now Send Email
            SmtpClient mailClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
            mailClient.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            mailClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_mailSettings.Mail);
            mailMessage.To.Add(hospitals.Email);

            mailMessage.Subject = "Thanks for registering with us  ";
            //var body = GetMailBodyWithButton(email);
            mailMessage.Body = "<h2>Your UserName is</h2> " + hospitals.UserName + "<br /> <h2>Your Password is </h2> " + strNewPassword;

            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.High;


            mailClient.Send(mailMessage);

            //Add the Username and Password to the Application User
            ApplicationUser user = new ApplicationUser()
            {
                UserName = hospitals.UserName,
                Email = hospitals.Email,
                
                PhoneNumber=hospitals.PhoneNumber,
              
            };
            var result = await _userManager.CreateAsync(user,strNewPassword);
            if (user.Role == null)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Hospital);
            }

            return hospitals ;
        }


        public string GeneratePassword()
        {
            string PasswordLength = "10";
            string NewPassword = "";

            string allowedChars = "";
            allowedChars = "0,1,2,3,4,5,6,7,8,9";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,!,@,#,$,%,^,&,*,~,?";

            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string IDString = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(PasswordLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                IDString += temp;
                NewPassword = IDString;
            }
            return NewPassword;
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$?_-"                        // non-alphanumeric
    };
            CryptoRandom rand = new CryptoRandom();
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

    }
}
