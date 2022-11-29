using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Registration_Login.Data;
using Registration_Login.Identity;
using Registration_Login.Models;
using Registration_Login.Models.ViewModels;
using Registration_Login.Service.IService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vonage;
using Vonage.Request;

namespace Registration_Login.Service
{
    public class Register : IRegister
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationSignInManager _applicationSignInManager;
        private readonly AppSettings _appSettings;
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISendGridService _sendGridService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public VonageCredentials _vonageCredentials { get; }
        public Register(RoleManager<IdentityRole> roleManager,IOptions<AppSettings> appSettings, ApplicationUserManager applicationUserManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ApplicationSignInManager applicationSignInManager, IOptions<VonageCredentials> vonageCredentials, ISendGridService sendGridService)
        {
            _context = context;
            _applicationSignInManager = applicationSignInManager;
            _applicationUserManager = applicationUserManager;
            _appSettings = appSettings.Value;
            _vonageCredentials = vonageCredentials.Value;
            _userManager = userManager;
            _sendGridService = sendGridService;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> Authenticate(LoginVM loginVM)
        {
            var user = await _applicationSignInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
            
            if (user.Succeeded)
            {
                var appicationUser = await _applicationUserManager.FindByNameAsync(loginVM.UserName);
                appicationUser.PasswordHash = "";

                //Role 
                if (await _applicationUserManager.IsInRoleAsync(appicationUser, UserRoles.Admin))
                    appicationUser.Role = UserRoles.Admin;
                if (await _applicationUserManager.IsInRoleAsync(appicationUser, UserRoles.Hospital))
                    appicationUser.Role = UserRoles.Hospital;
                if (await _applicationUserManager.IsInRoleAsync(appicationUser, UserRoles.Clinic))
                    appicationUser.Role = UserRoles.Clinic;
                if (await _applicationUserManager.IsInRoleAsync(appicationUser, UserRoles.Doctor))
                    appicationUser.Role = UserRoles.Doctor;
                if (await _applicationUserManager.IsInRoleAsync(appicationUser, UserRoles.Patient))
                    appicationUser.Role = UserRoles.Patient;
                //JWT Token Genrated

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, appicationUser.Id),
                  new Claim(ClaimTypes.Role, appicationUser.Role),
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                appicationUser.Token = tokenHandler.WriteToken(token);
                return appicationUser;
               
            }
            else
            {
                return null;
            }
        }

      
        public IEnumerable<ApplicationUser> GetUsers()
        {
            var userlist = _context.ApplicationUsers.ToList();
            var roles = _context.Roles.ToList();
            var userRole = _context.UserRoles.ToList();
            foreach (var user in userlist)

            {
                var RoleId = userRole.FirstOrDefault(ri => ri.UserId == user.Id).RoleId;   //what role assign to user
                user.Role = roles.FirstOrDefault(r => r.Id == RoleId).Name;                //role name of the user              
            }

            return userlist;
        }

        public async Task<ApplicationUser> UpdateUser(RegisterVM register)
        {

            var obj =await  _userManager.FindByEmailAsync(register.Email);
            if (obj == null)
                return null;
            else
            {
                var user = new ApplicationUser();
                obj.Id = obj.Id;
                obj.UserName = register.UserName;
                obj.Email = register.Email;
                obj.State = register.State;
                obj.City = register.City;
                obj.StreetAddress = register.StreetAddress;
                obj.Name = register.Name;
                obj.PhoneNumber = register.PhoneNumber;
                obj.PostalCode = register.PostalCode;
             _context.ApplicationUsers.Update(obj);
            _context.SaveChanges();
            return obj;
        }
    }

      
    }
}
