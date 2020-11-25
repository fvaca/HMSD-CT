using System;
namespace HMSD.APIGateway.Service.Interface
{
    public interface IEndpointCallerService
    {
        string CallServiceEnpoint(string address, string endpoint, string urlParameters = "");
    }
}
