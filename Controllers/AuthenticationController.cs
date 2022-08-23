using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SLBFEMS.Enums;
using SLBFEMS.Models;
using SLBFEMS.ViewModels.Authentication;
using SLBFEMS.Interfaces;

namespace SLBFEMS.Controllers
{
    [Route("auth")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IAuthService _authService;

        public AuthenticateController(UserManager<ApplicationUserModel> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context,
            IConfiguration configuration, ILogger<AuthenticateController> logger, IAuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/login
        ///     {
        ///        "userName": "sanjana",
        ///        "password": "$Sanjana1"
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns user data with JWT</response>
        /// <response code="401">Unothorized user</response>
        /// <response code="403">User doesn't have access to this endpoint</response>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponseViewModel>> Login([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null && !user.DeleteStatus && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!user.EmailConfirmed)
                    {
                        _logger.LogWarning(string.Format("{0} is tried loggin to the system with out confirming email.", user.UserName));
                        return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel { Status = "Error", Message = "Please verify your email!" });
                    }

                    if (user.DeleteStatus)
                    {
                        _logger.LogWarning(string.Format("{0} deleted user is tried loggin to the system.", user.UserName));
                        return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel { Status = "Error", Message = "Your account is deleted by admins. Please contact us ASAP!" });
                    }

                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.NIC),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                     );


                    _logger.LogInformation(string.Format("{0} logged in to the system", user.UserName));
                    return Ok(new LoginResponseViewModel
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Id = user.Id,
                        NIC = user.NIC,
                        Name = user.FirstName + ' ' + user.LastName,
                        Username = user.UserName,
                        Email = user.Email,
                        Role = (List<string>)userRoles,
                        expiration = DateTime.Now.AddDays(3)
                    });
                }
                _logger.LogWarning(string.Format("{0} Invalid login attempt.", model.UserName));
                return Unauthorized(new ResponseModel { Status = "Unauthorized", Message = "Username or password incorrect!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in POST: auth/login.", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Register as a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/register
        ///     
        ///     "userInfo": {
        ///         "nic": "993581089v,
        ///         "firstName": "sanjana",
        ///         "lastName": "sulakshana",
        ///         "email": "sanajnasw@gmail.com",
        ///         "phoneNumber": "0771994147",
        ///         "address": "12/1, jayalanka mawatha, ampitiya.",
        ///         "username": "sanjanasw",
        ///         "password": "$Sanjan1",
        ///         "role": 0
        ///         },
        ///     "jobSeekerData"?: {
        ///         "currentLat": "1.232323",
        ///         "currentLong": "2.32323",
        ///         "profession": "Software Engineer",
        ///         "affiliations": [
        ///           {
        ///             "start": "jan 2021",
        ///             "end": "feb 2021",
        ///             "location": "kandy",
        ///             "organization": "built apps",
        ///             "title": "Associate Software Engineer"
        ///           }
        ///          ],
        ///          "education": [
        ///           {
        ///             "start": "jan 2021",
        ///             "end": "feb 2021",
        ///             "name": "Dharmaraja college"
        ///           }
        ///          ],
        ///         "qualifications": [
        ///           "java",
        ///           "Angular",
        ///           "SQL"
        ///          ],
        ///         "cvFileName": "7917212142022-03-07-23-03-37.docx"
        ///        }
        ///       }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="201">Returns success message</response>
        /// <response code="400">Job seeker register request without providing job seeker data</response>
        /// <response code="409">Username or email already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ResponseModel>> Register([FromBody] RegisterUserViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.UserInfo.Role == UserRoles.User && model.JobSeekerData == null)
                    {
                        return BadRequest(new ResponseModel { Status = "Error", Message = "Job seekers must provide jobseekers data" });
                    }
                    var userExists = await _userManager.FindByNameAsync(model.UserInfo.Username);
                    var userEmailExists = await _userManager.FindByEmailAsync(model.UserInfo.Email);
                    if (userExists != null)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Username is already exists!" });
                    }
                    else if (userEmailExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create another account.", model.UserInfo.Email));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Email is already exists!" });
                    }

                    var NICInfo = _authService.GetNICInfo(model.UserInfo.NIC);

                    ApplicationUserModel user = new()
                    {
                        NIC = model.UserInfo.NIC,
                        UserName = model.UserInfo.Username,
                        FirstName = model.UserInfo.FirstName,
                        LastName = model.UserInfo.LastName,
                        Email = model.UserInfo.Email,
                        PhoneNumber = model.UserInfo.PhoneNumber,
                        Gender = NICInfo.Gender,
                        Birthday = NICInfo.Birthday,
                        Address = model.UserInfo.Address,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };
                    var result = await _userManager.CreateAsync(user, model.UserInfo.Password);

                    if (!result.Succeeded)
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                    await RefreshRolesTableAsync();

                    if (!string.IsNullOrEmpty(model.UserInfo.Role.ToString()) && model.UserInfo.Role == UserRoles.User)
                    {

                        if (await _roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.UserInfo.Role.ToString()) && model.UserInfo.Role == UserRoles.Admin)
                    {

                        if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.UserInfo.Role.ToString()) && model.UserInfo.Role == UserRoles.Officer)
                    {

                        if (await _roleManager.RoleExistsAsync(UserRoles.Officer.ToString()))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.Officer.ToString());
                        }
                    }

                    if (model.UserInfo.Role == UserRoles.User)
                    {
                        await _context.JobSeekerData.AddAsync(new JobSeekerDataModel
                        {
                            NIC = model.UserInfo.NIC,
                            CurrentLat = model.JobSeekerData.CurrentLat,
                            CurrentLong = model.JobSeekerData.CurrentLong,
                            Profession = model.JobSeekerData.Profession,
                            CvFileName = model.JobSeekerData.CvFileName

                        });

                        if (model.JobSeekerData.Affiliations.Count > 0)
                        {
                            foreach (var affilicate in model.JobSeekerData.Affiliations)
                            {
                                await _context.AffiliationData.AddAsync(new AffiliationDataModel
                                {
                                    NIC = model.UserInfo.NIC,
                                    Start = affilicate.Start,
                                    End = affilicate.End,
                                    Title = affilicate.Title,
                                    Location = affilicate.Location,
                                    Organization = affilicate.Organization,
                                });
                            }
                        }

                        if (model.JobSeekerData.Qualifications.Count > 0)
                        {
                            foreach (var qualification in model.JobSeekerData.Qualifications)
                            {
                                await _context.Qualifications.AddAsync(new QualificationsModel
                                {
                                    NIC = model.UserInfo.NIC,
                                    Qualification = qualification,
                                });
                            }
                        }

                        if (model.JobSeekerData.Education.Count > 0)
                        {
                            foreach (var education in model.JobSeekerData.Education)
                            {
                                await _context.EducationData.AddAsync(new EducationDataModel
                                {
                                    NIC = model.UserInfo.NIC,
                                    Start = education.Start,
                                    End = education.End,
                                    Name = education.Name
                                });
                            }
                        }
                    }

                    _context.SaveChanges();
                    await transaction.CommitAsync();


                    string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    _authService.SendEmail("verify", null, user, confirmationToken);
                    _logger.LogInformation(string.Format("{0} new {1} registered successfully!.", user.UserName, model.UserInfo.Role.ToString()));
                    return StatusCode(StatusCodes.Status201Created, new ResponseModel { Status = "Success", Message = "User created successfully!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Error occcured in POST: auth/register.", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/confirm-email
        ///     {
        ///         "userid": "gfuie-8feiufb-reufberf-rei",
        ///         "token": "kjufbkjdfuirefu8h4r94ruiuwb38dbnie844bu44bi"
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Invalid token</response>
        /// <response code="404">User not found</response>
        [HttpPost("confirm-email")]
        public async Task<ActionResult<ResponseModel>> ConfirmEmail([FromBody] ConfirmEmailViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                IdentityResult result = await _userManager.ConfirmEmailAsync(user, model.Token);
                if (!result.Succeeded)
                {
                    _logger.LogWarning(string.Format("{0} is tried to comfirm email with invalid token.", user.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
                }

                _authService.SendEmail("verified", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} successfully confirmed email!", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Verification successful, you can now login" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in POST: auth/confirm-email", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/forgot-password
        ///     {
        ///         "email": "sanjanasw99@gmail.com"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="404">User not found</response>
        [HttpPost("forgot-Password")]
        public async Task<ActionResult<ResponseModel>> ForgotPassword([FromBody] ForgetPasswordViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                _authService.SendEmail("resetPass", null, user, token);
                _logger.LogInformation(string.Format("{0} generated reset password link.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Reset Password Link Sent!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in POST: auth/forgot-password.", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/reset-password
        ///     {
        ///         "userid": "gfuie-8feiufb-reufberf-rei",
        ///         "token": "kjufbkjdfuirefu8h4r94ruiuwb38dbnie844bu44bi",
        ///         "password": "Not@1234"
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Invalid token</response>
        /// <response code="404">User not found</response>
        [HttpPut("reset-Password")]
        public async Task<ActionResult<ResponseModel>> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByIdAsync(model.Userid);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(string.Format("{0} is tried to reset password with invalid token.", user.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
                }

                _authService.SendEmail("resetted", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} successfully resetted password.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Password Reset Successfull!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in PUT: auth/reset-password", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /auth/change-password
        ///     {
        ///         "currentPassword": "Not@1234",
        ///         "newPassword": "1234@Not"
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Password doesn't meet minimum requirements</response>
        /// <response code="403">Current password incorrect</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpPut("change-password")]
        public async Task<ActionResult<ResponseModel>> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                var loggedInUsername = token.Claims.First(claim => claim.Type == "unique_name").Value;
                var user = await _userManager.FindByNameAsync(loggedInUsername);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
                {
                    _logger.LogWarning(string.Format("{0} tried to change password with incorrect current password.", user.UserName));
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel { Status = "Error", Message = "Current password is incorrect!" });
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Somethig went wrong!" });
                }

                _authService.SendEmail("passwordChanged", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} successfully changed password.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Password change Successfull!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in PUT: auth/change-password.", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Create new admin/officers account. [Access: Admins and Officers only]
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/force-onboard
        ///     
        ///     "userInfo": {
        ///         "nic": "string",
        ///         "firstName": "string",
        ///         "lastName": "string",
        ///         "email": "user@example.com",
        ///         "phoneNumber": "string",
        ///         "address": "string",
        ///         "username": "string",
        ///         "role": 0
        ///         }
        ///       }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="201">Returns new user details</response>
        /// <response code="400">Username or email already exists</response>
        /// <response code="403">Forbidden</response>
        /// <response code="409">User details conflict</response>
        /// <response code="500">Internal server error</response>
        [Authorize(Roles = "Admin, Officers")]
        [HttpPost]
        [Route("force-onboard")]
        public async Task<ActionResult> NewUser([FromBody] NewUserViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Role == UserRoles.User)
                    {
                        return BadRequest();
                    }

                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                    var loggedInUsername = token.Claims.First(claim => claim.Type == "unique_name").Value;

                    var userExists = await _userManager.FindByNameAsync(model.Username);
                    var userEmailExists = await _userManager.FindByEmailAsync(model.Email);

                    if (userExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create force-onboard account for existing username: {1}.", loggedInUsername, model.Username));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Username is already exists!" });
                    }
                    else if (userEmailExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create force-onboard account for existing email: {1}.", loggedInUsername, model.Email));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Email is already exists!" });
                    }

                    ApplicationUserModel user = new()
                    {
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };

                    var result = await _userManager.CreateAsync(user, "$NewUserPassword1Temp");
                    if (!result.Succeeded)
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                    await RefreshRolesTableAsync();

                    if (!string.IsNullOrEmpty(model.Role.ToString()) && model.Role == UserRoles.Admin)
                    {

                        if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.Role.ToString()) && model.Role == UserRoles.Officer)
                    {

                        if (await _roleManager.RoleExistsAsync(UserRoles.Officer.ToString()))
                        {
                            await _userManager.AddToRoleAsync(user, UserRoles.Officer.ToString());
                        }
                    }

                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await transaction.CommitAsync();

                    _logger.LogInformation(string.Format("{0}, new user onborded to username: {1}", loggedInUsername, user.UserName));
                    _authService.SendEmail("newUser", null, user, resetToken);
                    return StatusCode(StatusCodes.Status201Created, new ResponseModel { Status = "Success", Message = "User created successfully!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Error occcured in POST: auth/force-onboard.", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// New user account setup
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/new-user-setup
        ///     {
        ///         "userid": "gfuie-8feiufb-reufberf-rei",
        ///         "token": "kjufbkjdfuirefu8h4r94ruiuwb38dbnie844bu44bi",
        ///         "password": "Not@1234"
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Invalid token or password doesn't meet minimum requirements</response>
        /// <response code="404">User not found</response>
        [HttpPost("new-user-setup")]
        public async Task<ActionResult<ResponseModel>> NewUserSetup([FromBody] ResetPasswordViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByIdAsync(model.Userid);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(string.Format("{0} is tried to setup fresh account with invalid token.", user.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Invalid token or password doesn't meet minimum requirements!" });
                }

                _authService.SendEmail("newUserSetup", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} is onboarded successfully.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "New User Setup Successfull!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in POST: auth/new-user-setup", ex.Message);
                throw;
            }
        }


        private async Task RefreshRolesTableAsync()
        {
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Officer.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Officer.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));
        }

    }
}
