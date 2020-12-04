using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using HMSD.APIGateway.Service.Interface;
using Microsoft.Extensions.Logging;

namespace HMSD.APIGateway.Service
{
    public class EndpointCallerService : IEndpointCallerService
    {
        static HttpClient client = new HttpClient();
        private readonly ILogger _logger;

        public EndpointCallerService(ILogger<EndpointCallerService> logger)
        {
            _logger = logger;
        }
        
        public string CallServiceEnpoint(string baseaddress, string endpoint, string urlparameters = "")
        {
            var response = RunAsync<string>(baseaddress + endpoint, urlparameters, _logger).GetAwaiter().GetResult();            
            return response;

        }

        public static async Task<string> GetAsync<T>(string url, string urlparameters, ILogger _logger)
        {            
            using (var httpClient = new HttpClient())
            {
                _logger.LogInformation($"CallServiceEnpoint::RunAsync::GetAsync [urlparameters: {urlparameters}]");
                using (var response = await httpClient.GetAsync(url + urlparameters))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                        return apiResponse;

                    throw new HttpResponseException($"|||| CallServiceEnpoint::RunAsync::GetAsync [urlparameters: {urlparameters}] ||| \n" + apiResponse) { Status = (int)response.StatusCode };
                
                }
            }
        }

        public static async Task<string> RunAsync<T>(string url, string urlParameters, ILogger _logger)
        {
            return await GetAsync<T>(url, urlParameters, _logger);
        }

       
    }
}
