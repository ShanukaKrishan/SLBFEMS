using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SLBFEMS.Enums;
using SLBFEMS.Models;
using SLBFEMS.ViewModels.Complaint;

namespace SLBFEMS.Controllers
{
    [Authorize]
    [Route("complaint")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    public class ComplaintController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public ComplaintController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get complaints
        /// </summary>
        /// <remarks>
        /// Todo: Add description    nullable parameter. if parameter is null returns all complaints
        /// </remarks>
        /// <response code="200">Returns complaint</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintViewModel>>> GetComplaint([FromQuery] int? id = null)
        {
            try
            {
                var complaints = new List<ComplaintViewModel>();
                if (id == null)
                {
                    var complaintsList = await _context.Complaints.ToListAsync();
                    foreach (var complaint in complaintsList)
                    {
                        complaints.Add(new ComplaintViewModel
                        {
                            ComplaintStatus = complaint,
                            MessageThred = await _context.ComplaintMessages.Where(x => x.ComplaintId == complaint.Id).Select(x => new ComplaintMessagesViewModel
                            {
                                Complaint = x.Message,
                                Nic = x.Nic,
                                TimeStamp = x.TimeStamp,
                            }).ToListAsync()
                        });
                    }

                    return Ok(complaints);
                }
                else
                {
                    return Ok(new ComplaintViewModel
                    {
                        ComplaintStatus = await _context.Complaints.FirstOrDefaultAsync(x => x.Id == id),
                        MessageThred = await _context.ComplaintMessages.Where(x => x.ComplaintId == id).Select(x => new ComplaintMessagesViewModel
                        {
                            Complaint = x.Message,
                            Nic = x.Nic,
                            TimeStamp = x.TimeStamp,
                        }).ToListAsync()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in GET: complaint.", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Make complaint
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /complaint
        ///     {
        ///         "complaint": "your complaint"
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ResponseModel>> PostComplaint([FromBody] ComplaintCreateViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                    var loggedInUserNic = token.Claims.First(claim => claim.Type == "nameid").Value;

                    var complaint = new ComplaintDataModel
                    {
                        Status = ComplaintStatus.New

                    };

                    await _context.Complaints.AddAsync(complaint);

                    await _context.SaveChangesAsync();

                    await _context.ComplaintMessages.AddAsync(new ComplaintMessagesModel
                    {
                        ComplaintId = complaint.Id,
                        Message = model.Complaint,
                        Nic = loggedInUserNic
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(new ResponseModel { Status = "Success", Message = "Complaint creation successful" });

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Error occcured in POST: complint.", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update complaint
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /complaint/1
        ///     {
        ///         "complaint": "your reply"
        ///         "isComplete": true
        ///     }
        ///
        /// Todo: Add description
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="404">Complaint or User not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel>> PutComplaint([FromRoute] int id, [FromBody] ComplaintUpdateViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var complaint = await _context.Complaints.FirstOrDefaultAsync(x => x.Id == id);

                    if (complaint == null)
                        return NotFound();

                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                    var loggedInUserNic = token.Claims.First(claim => claim.Type == "nameid").Value;

                    if (model.IsComplete)
                    {
                        complaint.Status = ComplaintStatus.Resolved;
                        complaint.CompletedAt = DateTime.Now;
                    }
                    else
                    {
                        complaint.Status = ComplaintStatus.Replyed;
                    }
                    _context.Entry(complaint).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    await _context.ComplaintMessages.AddAsync(new ComplaintMessagesModel
                    {
                        ComplaintId = complaint.Id,
                        Message = model.Complaint,
                        Nic = loggedInUserNic
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(new ResponseModel { Status = "Success", Message = "Complaint update successful" });

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Error occcured in PUT complaint/id", ex.Message);
                    throw;
                }
            }
        }
    }
}
