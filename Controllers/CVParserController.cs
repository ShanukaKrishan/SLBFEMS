using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using FileUploadTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLBFEMS.Enums;
using SLBFEMS.Interfaces;
using SLBFEMS.Models;
using SLBFEMS.ViewModels.CVParser;

namespace SLBFEMS.Controllers
{
    [Route("cv-prase")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
    public class CVParserController : ControllerBase
    {
        private readonly IFileManager _fileManager;
        private readonly ICVParserService _cvParser;
        private readonly ILogger _logger;

        public CVParserController(IFileManager fileManager, ICVParserService cvParser, ILogger<CVParserController> logger)
        {
            _fileManager = fileManager;
            _cvParser = cvParser;
            _logger = logger;
        }

        /// <summary>
        /// Upload and parse CV
        /// </summary>
        /// <remarks>
        /// Accepted formats : docx / pdf
        ///
        /// </remarks>
        /// <response code="200">Returns cv data with uploaded file name</response>
        /// <response code="400">Unsupported file format</response>
        /// <response code="500">Something went wrong during cv prasing</response>
        [HttpPost]
        public async Task<ActionResult<CVParserResponseViewModel>> Upload([FromForm] FileModel model)
        {
            try
            {
                if(Request.Form.Files[0] != null)
                {
                    model.File = Request.Form.Files[0];
                }
                if (model.File != null)
                {
                    var fileFormat = Path.GetExtension(model.File.FileName);
                    if (fileFormat == ".docx" || fileFormat == ".pdf")
                    {
                        var response = await _fileManager.Upload(model, FileCategories.CV);
                        var cvResponse = await _cvParser.GetCvData(response.URL);
                        if (cvResponse.IsSuccessful)
                        {
                            return Ok(new CVParserResponseViewModel
                            {
                                CVData = cvResponse.CvData,
                                FileName = response.FileName
                            });
                        }

                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Something went wrong during cv prasing" });
                    }
                }
                return BadRequest(new ResponseModel { Status = "Error", Message = "Unsupported file format" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occcured in POST: cv-parser.", ex.Message);
                throw;
            }
        }
    }
}
