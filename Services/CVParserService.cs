using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SLBFEMS.Interfaces;
using SLBFEMS.ViewModels.CVParser;

namespace SLBFEMS.Services
{
    public class CVParserService : ICVParserService
    {
        public IConfiguration _configuration { get; }
        private readonly ILogger<CVParserService> _logger;

        public CVParserService(IConfiguration configuration, ILogger<CVParserService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<CVResponseVIewModel> GetCvData(string url)
        {
            try
            {
                var options = new RestClientOptions("https://api.apilayer.com/resume_parser/url")
                {
                    Timeout = -1
                };
                var client = new RestClient(options);
                var request = new RestRequest();
                request.AddQueryParameter("url", url);
                request.AddHeader("apikey", _configuration.GetValue<string>("CVParserApiKey"));

                var response = await client.GetAsync(request);
                var output = JsonConvert.DeserializeObject<CVParserExternalResponseViewModel>(response.Content);
                return new CVResponseVIewModel
                {
                    IsSuccessful = true,
                    CvData = output
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured in cv prasing", ex.Message);
                return new CVResponseVIewModel
                {
                    IsSuccessful = false,
                };
            }
        }
    }
}
