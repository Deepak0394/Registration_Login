
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Registration_Login.Data;
using Registration_Login.Identity;
using Registration_Login.Models;
using Registration_Login.Models.ViewModels;
using Registration_Login.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vonage;
using Vonage.Request;
using Vonage.Verify;

namespace Registration_Login.Controllers
{
    [Route("api/Registration")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IRegister _userServie;
        private readonly IMailService _mailService;
        private readonly ISendGridService _sendGridService;
        public VonageCredentials _vonageCredentials { get; }
        public RegisterController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IRegister userServie, IOptions<VonageCredentials> vonageCredentials, 
            IMailService mailService, ISendGridService sendGridService)
        {
            _context = context;
            _userManager = userManager;
            _userServie = userServie;
            _vonageCredentials = vonageCredentials.Value;
            _mailService = mailService;
            _sendGridService = sendGridService;
            _roleManager = roleManager;

        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("UserList")]
       
        public async Task<IEnumerable<ApplicationUser>> GetUserList()
        {
            return _userServie.GetUsers();
        }

        [HttpGet("RolesList")]

        public IActionResult GetRolelist()
        {
            return Ok(_context.Roles.ToList());

        }




        [HttpPost("RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterVM register)
        {
            if (!ModelState.IsValid)
            {
                return Ok("Fill Valid Data");
            }
            else
            {
                var existingUser = await _userManager.FindByEmailAsync(register.Email);
                if (existingUser == null)
                {
                    ApplicationUser user = new ApplicationUser()
                    {

                        Name = register.Name,
                        StreetAddress = register.StreetAddress,
                        State = register.State,
                        City = register.City,
                        PostalCode = register.PostalCode,
                        UserName = register.UserName,
                        Email = register.Email,
                        PhoneNumber = register.PhoneNumber,
                        Role=register.roleId
                       
                    };
                 

                    var result = await _userManager.CreateAsync(user, register.Password);
                    if (!result.Succeeded)

                        return BadRequest("Error ,User creation failed! Please check user details and try again.");


                    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))

                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                    if (!await _roleManager.RoleExistsAsync(UserRoles.Hospital))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Hospital));

                    if (!await _roleManager.RoleExistsAsync(UserRoles.Clinic))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Clinic));

                    if (!await _roleManager.RoleExistsAsync(UserRoles.Patient))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Patient));

                    if (!await _roleManager.RoleExistsAsync(UserRoles.Doctor))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Doctor));


                    // Only for First User  
                            //  await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                    

                    if (user.Role == null )
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.Patient);
                    }
                    if (user.Role != null)
                    {
                        await _userManager.AddToRoleAsync(user, register.roleId);
                    }





                    //Send Message
                   /* var credentials = Credentials.FromApiKeyAndSecret(
                      _vonageCredentials.APIKey,
                      _vonageCredentials.APISecret
                        );
                    var VonageClient = new VonageClient(credentials);




                    var response = VonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
                    {
                        To = user.PhoneNumber,
                        From = _vonageCredentials.PhoneNumber,
                        Text = "You have Register Successfully ",
                        Title = "Vonage Sms",
                        Body = "Thank you for register with use ."

                    });*/

                    //Email Send 
                    _mailService.SendEmailAsync(register.Email);
                   // _sendGridService.SendEmailAsync(register.Email);

                    return Ok(result);
                    _context.SaveChanges();
                    return Ok(user);
                }
                return Ok(existingUser);
            }

            return BadRequest(" ");



        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> login(LoginVM loginVM)
        {
            var user = await _userServie.Authenticate(loginVM);
            if (user == null)
                return BadRequest(new { message = "Wrong Email or Password" });
            return Ok(user);
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteRegister(string REmail)
        {
            var UserInDb = await _userManager.FindByEmailAsync(REmail);
            if (UserInDb == null)
            {
                return BadRequest(new { message = "User Not Found" });
            }
            else
                _context.ApplicationUsers.Remove(UserInDb);
            _context.SaveChanges();
              
            
            return Ok(UserInDb);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> Update(RegisterVM register)
        {
            if (!ModelState.IsValid && register == null)
            {
                return BadRequest("Enter Valid Data");
            }
            else
            {
                var result = await _userServie.UpdateUser(register);
                return Ok(result);
            }
            }




    }
}
