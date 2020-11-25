using System;
namespace HMSD.EncryptionService.Services.Interface
{
    public interface IKeyRotatorService
    {
        string GetActiveKey();
    }
}
