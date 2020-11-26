using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
            var response = RunAsync<string>(baseaddress + endpoint, urlparameters).GetAwaiter().GetResult();            
            return response;

        }

        //private static HttpClient GetHttpClient(string url)
        //{
        //    var client = new HttpClient { BaseAddress = new Uri(url) };
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    return client;
        //}

        //private static async Task<string> GetAsync<T>(string url, string urlparameters)
        //{
        //    try
        //    {
        //        using (var client = GetHttpClient(url))
        //        {
        //            HttpResponseMessage response = await client.GetAsync(urlparameters);
        //            if (response.StatusCode == HttpStatusCode.OK)
        //            {
        //                var result = await response.Content.ReadAsStringAsync();
        //                return result;
        //            }

        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }
        //}

        public static async Task<string> GetAsync<T>(string url, string urlparameters)
        {            
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url+urlparameters))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return apiResponse;
                }
            }
        }

        public static async Task<string> RunAsync<T>(string url, string urlParameters)
        {
            return await GetAsync<T>(url, urlParameters);
        }

       
    }
}
