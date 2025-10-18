using System.Security.Cryptography;

namespace Application_Form.Application.Services
{
    public class ApiCredentialService : IApiCredentialService
    {
        public Task<ApiCredentials> GenerateAsync(CancellationToken cancellationToken = default)
        {
            // Simple credential generation using GUIDs + randomness. Replace with secure provider as needed.
            var apiKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var clientId = Guid.NewGuid().ToString("N");
            var clientSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var creds = new ApiCredentials(apiKey, clientId, clientSecret);
            return Task.FromResult(creds);
        }
    }
}
